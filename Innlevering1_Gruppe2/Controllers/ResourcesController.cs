using System.Security.Claims;
using System.Globalization;
using Reachout.DataAccess.Repositories;
using Reachout.Models.Entities;
using Reachout.DataAccess;
using Reachout.Models;
using Reachout.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Controllers;
[Authorize(Roles = "Private,Public")]
public class ResourcesController : Controller
{
    private readonly ApplicationDbContext context;
    private readonly WorkflowService workflow;
    private readonly IResourceRepository resourceRepository;
    public ResourcesController(ApplicationDbContext context, WorkflowService workflow, IResourceRepository resourceRepository)
    {
        this.context = context;
        this.workflow = workflow;
        this.resourceRepository = resourceRepository;
    }

    private int AccountId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Details(int id)
    {
        var resource = await context.Resources.Include(resource => resource.Location).Include(resource => resource.ResourceType).Include(resource => resource.ProviderAccount).ThenInclude(provider => provider.Account).AsNoTracking().SingleOrDefaultAsync(resource => resource.resource_id == id);
        if (resource == null || User.IsInRole("Private") && resource.account_id != AccountId)
        {
            return NotFound();
        }

        if (resource.archived && resource.account_id != AccountId && !await context.Reservations.AnyAsync(reservation => reservation.ResourceMatch.resource_id == id && reservation.public_actor_account_id == AccountId))
        {
            return NotFound();
        }

        return View(new ResourceDetailsViewModel { Resource = resource, CanEdit = User.IsInRole("Private") && !resource.archived, Periods = await context.AvailabilityPeriods.AsNoTracking().Where(period => period.resource_id == id).OrderBy(period => period.available_from).ToListAsync(), Needs = User.IsInRole("Public") ? await context.ResourceNeeds.AsNoTracking().Where(need => need.public_actor_account_id == AccountId && !need.archived && need.status != "Løst" && need.resource_type_id == resource.resource_type_id && need.required_end > DateTime.Now && (!need.deadline.HasValue || need.deadline >= DateTime.Now) && !context.ResourceMatches.Any(match => match.resource_need_id == need.resource_need_id)).ToListAsync() : new() });
    }

    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Photo(int id)
    {
        var resource = await context.Resources.AsNoTracking().SingleOrDefaultAsync(resource => resource.resource_id == id);
        if (resource == null || resource.image_data == null || User.IsInRole("Private") && resource.account_id != AccountId)
        {
            return NotFound();
        }

        if (resource.archived && resource.account_id != AccountId && !await context.Reservations.AnyAsync(reservation => reservation.ResourceMatch.resource_id == id && reservation.public_actor_account_id == AccountId))
        {
            return NotFound();
        }

        Response.Headers["X-Content-Type-Options"] = "nosniff";
        Response.Headers["Content-Security-Policy"] = "default-src 'none'; sandbox";
        return File(resource.image_data, resource.image_content_type!);
    }

    [Authorize(Roles = "Private"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(int id, bool restore = false)
    {
        TempData["Message"] = await workflow.ArchiveResourceAsync(id, AccountId, restore) ?? (restore ? "Ressursen er gjenopprettet." : "Ressursen er arkivert.");
        return RedirectToAction(nameof(MyContributions), new { archived = !restore });
    }

    [Authorize(Roles = "Private"), HttpGet]
    public async Task<IActionResult> Register()
    {
        await LoadRegistrationOptionsAsync();
        return View(new ResourceEditViewModel());
    }

    [Authorize(Roles = "Private"), HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<IActionResult> Register(ResourceEditViewModel model)
    {
        if (model.End <= model.Start || model.End <= DateTime.Now)
        {
            ModelState.AddModelError("End", "Velg en gyldig tilgjengelig periode.");
        }

        if (!await ResourceRegistrationTypes.AnyAsync(type => type.resource_type_id == model.ResourceTypeId))
        {
            ModelState.AddModelError("ResourceTypeId", "Velg en gyldig ressurskategori. Frivillighet registreres gjennom frivilligprofilen.");
        }

        if (model.AvailabilityStatus is not ("Tilgjengelig nå" or "Etter avtale" or "Ikke tilgjengelig"))
        {
            ModelState.AddModelError("AvailabilityStatus", "Velg gyldig tilgjengelighet.");
        }

        (byte[] Data, string Type)? photo = null;
        if (model.Photo != null)
        {
            try
            {
                photo = await PhotoUpload.ReadAsync(model.Photo);
            }
            catch (ArgumentException error)
            {
                ModelState.AddModelError("Photo", error.Message);
            }
        }

        if (!ModelState.IsValid)
        {
            await LoadRegistrationOptionsAsync();
            return View(model);
        }

        var id = GetCurrentAccountId()!.Value;
        var account = await context.Accounts.AsNoTracking().SingleAsync(account => account.account_id == id);
        var resource = new Resource
        {
            account_id = id,
            resource_name = model.Name.Trim(),
            description = model.Description ?? string.Empty,
            resource_type_id = model.ResourceTypeId!.Value,
            availability_status = model.AvailabilityStatus,
            verification_status = "Ikke verifisert",
            contact_details = $"Navn: {account.contact_name}, Telefon: {account.phone}",
            image_data = photo?.Data,
            image_content_type = photo?.Type,
            Location = new Location
            {
                address_or_area = string.IsNullOrWhiteSpace(model.Address) ? FormatCoordinates(model.Latitude!.Value, model.Longitude!.Value) : model.Address.Trim(),
                latitude = (decimal)model.Latitude!.Value,
                longitude = (decimal)model.Longitude!.Value
            }
        };
        await using var transaction = await context.Database.BeginTransactionAsync();
        await resourceRepository.AddAsync(resource);
        context.AvailabilityPeriods.Add(new Availability { resource_id = resource.resource_id, available_from = model.Start, available_to = model.End });
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return RedirectToAction(nameof(Registered), new { id = resource.resource_id });
    }

    [Authorize(Roles = "Private")]
    [HttpGet]
    public async Task<IActionResult> MyContributions(bool archived = false)
    {
        var accountId = GetCurrentAccountId();
        if (accountId is null)
        {
            return Challenge();
        }

        var resources = await resourceRepository.GetByAccountIdAsync(accountId.Value);
        ViewBag.Archived = archived;
        ViewBag.PendingRequests = await context.Reservations.CountAsync(reservation => reservation.reservation_status == "Venter på svar" && reservation.ResourceMatch.Resource.account_id == accountId.Value);
        ViewBag.HasVolunteerProfile = await context.VolunteerProfiles.AnyAsync(profile => profile.account_id == accountId.Value);
        ViewBag.PendingInterests = await context.VolunteerInterests.CountAsync(interest => interest.interest_status == "Venter på svar" && interest.VolunteerProfile.account_id == accountId.Value);
        return View(resources.Where(resource => resource.archived == archived).ToList());
    }

    [Authorize(Roles = "Private")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var accountId = GetCurrentAccountId();
        if (accountId is null)
        {
            return Challenge();
        }

        var resource = await resourceRepository.GetByIdForAccountAsync(id, accountId.Value);
        if (resource is null)
        {
            return NotFound();
        }

        await LoadRegistrationOptionsAsync();
        return View(new ResourceEditViewModel { Id = resource.resource_id, Name = resource.resource_name, Description = resource.description, Address = resource.Location.address_or_area, AvailabilityStatus = resource.availability_status, HasPhoto = resource.image_data != null, ResourceTypeId = resource.resource_type_id, Latitude = (double)resource.Location.latitude, Longitude = (double)resource.Location.longitude });
    }

    [Authorize(Roles = "Private")]
    [HttpPost, RequestSizeLimit(3 * 1024 * 1024)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ResourceEditViewModel model)
    {
        var accountId = GetCurrentAccountId();
        if (accountId is null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            await LoadRegistrationOptionsAsync();
            return View(model);
        }

        var validResourceType = await ResourceRegistrationTypes.AnyAsync(type => type.resource_type_id == model.ResourceTypeId!.Value);
        if (!validResourceType)
        {
            ModelState.AddModelError(nameof(model.ResourceTypeId), "Velg en gyldig ressurskategori. Frivillighet registreres gjennom frivilligprofilen.");
            await LoadRegistrationOptionsAsync();
            return View(model);
        }

        await using var transaction = await context.Database.BeginTransactionAsync();
        await context.Database.ExecuteSqlRawAsync("SELECT Id FROM AppWriteLock WHERE Id=1 FOR UPDATE");
        var resource = await resourceRepository.GetByIdForAccountAsync(model.Id, accountId.Value);
        if (resource is null)
        {
            return NotFound();
        }

        var account = await context.Accounts.AsNoTracking().SingleOrDefaultAsync(account => account.account_id == accountId.Value);
        if (account is null)
        {
            return Challenge();
        }

        if (resource.archived)
        {
            return BadRequest("Gjenopprett ressursen før redigering.");
        }

        if (model.AvailabilityStatus is not ("Tilgjengelig nå" or "Etter avtale" or "Ikke tilgjengelig"))
        {
            return BadRequest();
        }

        if (model.ResourceTypeId != resource.resource_type_id && await context.Reservations.AnyAsync(reservation => reservation.ResourceMatch.resource_id == model.Id && (reservation.reservation_status == "Venter på svar" || reservation.reservation_status == "Godtatt")))
        {
            ModelState.AddModelError("ResourceTypeId", "Avklar aktive forespørsler før du endrer ressursens kategori.");
            await LoadRegistrationOptionsAsync();
            return View(model);
        }

        if (model.AvailabilityStatus == "Ikke tilgjengelig" && await context.Reservations.AnyAsync(reservation => reservation.ResourceMatch.resource_id == model.Id && reservation.reservation_status == "Godtatt"))
        {
            ModelState.AddModelError("AvailabilityStatus", "Avklar bekreftede reservasjoner først.");
            await LoadRegistrationOptionsAsync();
            return View(model);
        }

        if (model.Photo != null)
        {
            try
            {
                var photo = await PhotoUpload.ReadAsync(model.Photo);
                resource.image_data = photo.Data;
                resource.image_content_type = photo.Type;
            }
            catch (ArgumentException error)
            {
                ModelState.AddModelError("Photo", error.Message);
                await LoadRegistrationOptionsAsync();
                return View(model);
            }
        }
        else if (model.RemovePhoto)
        {
            resource.image_data = null;
            resource.image_content_type = null;
        }

        resource.availability_status = model.AvailabilityStatus;
        resource.resource_name = model.Name.Trim();
        resource.description = model.Description?.Trim() ?? string.Empty;
        resource.resource_type_id = model.ResourceTypeId!.Value;
        resource.contact_details = $"Navn: {account.contact_name}, Telefon: {account.phone}";
        resource.Location.latitude = (decimal)model.Latitude!.Value;
        resource.Location.longitude = (decimal)model.Longitude!.Value;
        resource.Location.address_or_area = string.IsNullOrWhiteSpace(model.Address) ? FormatCoordinates(model.Latitude.Value, model.Longitude.Value) : model.Address.Trim();
        await resourceRepository.UpdateAsync(resource);
        await transaction.CommitAsync();
        return RedirectToAction(nameof(MyContributions));
    }

    [Authorize(Roles = "Private")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var accountId = GetCurrentAccountId();
        if (accountId is null)
        {
            return Challenge();
        }

        var resource = await resourceRepository.GetByIdForAccountAsync(id, accountId.Value);
        if (resource == null)
        {
            return NotFound();
        }

        if (await context.ResourceMatches.AnyAsync(match => match.resource_id == id))
        {
            TempData["Message"] = "Ressursen har forespørselshistorikk. Bruk arkivering i stedet.";
            return RedirectToAction(nameof(MyContributions));
        }

        context.AvailabilityPeriods.RemoveRange(context.AvailabilityPeriods.Where(period => period.resource_id == id));
        await resourceRepository.DeleteAsync(id, accountId.Value);
        return RedirectToAction(nameof(MyContributions));
    }

    private IQueryable<ResourceType> ResourceRegistrationTypes => context.ResourceTypes
        .Where(type => type.category_name != "Frivillige" && type.category_name != "Frivillig");

    private async Task LoadRegistrationOptionsAsync()
    {
        ViewBag.ResourceTypes = await ResourceRegistrationTypes.AsNoTracking().OrderBy(type => type.category_name).ToListAsync();
    }

    private int? GetCurrentAccountId()
    {
        var accountIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(accountIdValue, out var accountId) ? accountId : null;
    }

    private static string FormatCoordinates(double latitude, double longitude)
    {
        return string.Format(CultureInfo.InvariantCulture, "Koordinater: {0:F6}, {1:F6}", latitude, longitude);
    }

    // Viser en enkel kvittering etter at leverandøren har registrert en ressurs.
    [Authorize(Roles = "Private,Public"), HttpGet]
    public async Task<IActionResult> Registered(int? id)
    {
        if (User.IsInRole("Public"))
        {
            return id.HasValue ? RedirectToAction("Details", "Resources", new { id }) : RedirectToAction("Index", "Map");
        }

        var accountId = GetCurrentAccountId();
        if (accountId is null)
        {
            return Challenge();
        }

        if (id is null)
        {
            return RedirectToAction(nameof(MyContributions));
        }

        var resource = await resourceRepository.GetByIdForAccountAsync(id.Value, accountId.Value);
        if (resource is null)
        {
            return NotFound();
        }

        return View(new ResourceRegistrationResultViewModel
        {
            Id = resource.resource_id,
            Name = resource.resource_name
        });
    }


}
