using System.Security.Claims;
using Reachout.DataAccess;
using Reachout.Models;
using Reachout.Models.Entities;
using Reachout.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Controllers;
[Authorize(Roles = "Private,Public")]
public class NeedsController : Controller
{
    private readonly ApplicationDbContext context;
    private readonly WorkflowService workflow;
    public NeedsController(ApplicationDbContext context, WorkflowService workflow)
    {
        this.context = context;
        this.workflow = workflow;
    }

    private int AccountId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(bool archived = false)
    {
        ViewBag.Archived = archived;
        if (User.IsInRole("Public"))
        {
            ViewBag.PendingRequests = await context.Reservations.CountAsync(reservation => reservation.reservation_status == "Venter på svar" && reservation.public_actor_account_id == AccountId);
            ViewBag.PendingInterests = await context.VolunteerInterests.CountAsync(interest => interest.interest_status == "Venter på svar" && interest.ResourceNeed.public_actor_account_id == AccountId);
        }

        var query = context.ResourceNeeds.Include(need => need.ResourceType).Include(need => need.Location).AsNoTracking();
        query = User.IsInRole("Private") ? query.Where(need => !need.archived && need.status != "Løst" && need.required_end > DateTime.Now && (!need.deadline.HasValue || need.deadline >= DateTime.Now)) : query.Where(need => need.public_actor_account_id == AccountId && need.archived == archived);
        return View(await query.OrderByDescending(need => need.priority == "Haster").ThenBy(need => need.deadline).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var need = await context.ResourceNeeds.Include(need => need.Location).Include(need => need.ResourceType).Include(need => need.PublicActorAccount).ThenInclude(actor => actor.Account).AsNoTracking().SingleOrDefaultAsync(need => need.resource_need_id == id);
        if (need == null || need.archived && need.public_actor_account_id != AccountId)
        {
            return NotFound();
        }

        var owner = User.IsInRole("Public") && need.public_actor_account_id == AccountId;
        var resources = User.IsInRole("Public") ? await context.Resources.Include(resource => resource.ResourceType).AsNoTracking().Where(resource => !resource.archived && resource.availability_status != "Ikke tilgjengelig" && resource.resource_type_id == need.resource_type_id).Select(resource => new Resource { resource_id = resource.resource_id, resource_name = resource.resource_name, ResourceType = resource.ResourceType }).Take(20).ToListAsync() : new();
        var myProfile = User.IsInRole("Private") ? await context.VolunteerProfiles.AsNoTracking().SingleOrDefaultAsync(profile => profile.account_id == AccountId) : null;
        var myInterest = myProfile == null ? null : await context.VolunteerInterests.AsNoTracking().FirstOrDefaultAsync(interest => interest.volunteer_profile_id == myProfile.volunteer_profile_id && interest.resource_need_id == id && (interest.interest_status == "Venter på svar" || interest.interest_status == "Godtatt"));
        return View(new NeedDetailsViewModel { Need = need, MyVolunteerProfile = myProfile, MyActiveInterest = myInterest, CanEdit = owner && !need.archived && User.FindFirstValue("Permission") != "Lesetilgang", Resources = resources, Reservations = owner ? await context.Reservations.Include(reservation => reservation.ResourceMatch).ThenInclude(match => match.Resource).Include(reservation => reservation.ResourceMatch.ResourceNeed).AsNoTracking().Where(reservation => reservation.ResourceMatch.resource_need_id == id).ToListAsync() : new(), Interests = owner ? await context.VolunteerInterests.Include(interest => interest.ResourceNeed).Include(interest => interest.VolunteerProfile).ThenInclude(profile => profile.ProviderAccount).ThenInclude(provider => provider.Account).AsNoTracking().Where(interest => interest.resource_need_id == id).ToListAsync() : new() });
    }

    [Authorize(Roles = "Public"), HttpGet]
    public async Task<IActionResult> Create()
    {
        return View("Form", new NeedFormViewModel { Types = await context.ResourceTypes.AsNoTracking().ToListAsync() });
    }

    [Authorize(Roles = "Public"), HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var need = await context.ResourceNeeds.Include(need => need.Location).AsNoTracking().SingleOrDefaultAsync(need => need.resource_need_id == id && need.public_actor_account_id == AccountId && !need.archived);
        if (need == null)
        {
            return NotFound();
        }

        return View("Form", new NeedFormViewModel { Id = id, Title = need.title, Description = need.description, Priority = need.priority, ResourceTypeId = need.resource_type_id, Address = need.Location.address_or_area, Latitude = (double)need.Location.latitude, Longitude = (double)need.Location.longitude, Start = need.required_start, End = need.required_end, Deadline = need.deadline, Types = await context.ResourceTypes.AsNoTracking().ToListAsync() });
    }

    [Authorize(Roles = "Public"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(NeedFormViewModel model)
    {
        if (model.End <= model.Start || model.End <= DateTime.Now)
        {
            ModelState.AddModelError("End", "Velg en gyldig fremtidig periode.");
        }

        if (model.Deadline > model.Start)
        {
            ModelState.AddModelError("Deadline", "Svarfristen må være før behovet starter.");
        }

        if (model.Priority is not ("Normal" or "Haster" or "Planlagt"))
        {
            ModelState.AddModelError("Priority", "Velg gyldig prioritet.");
        }

        if (!await context.ResourceTypes.AnyAsync(type => type.resource_type_id == model.ResourceTypeId))
        {
            ModelState.AddModelError("ResourceTypeId", "Velg en gyldig kategori.");
        }

        if (!ModelState.IsValid)
        {
            model.Types = await context.ResourceTypes.AsNoTracking().ToListAsync();
            return View("Form", model);
        }

        await using var transaction = await context.Database.BeginTransactionAsync();
        await context.Database.ExecuteSqlRawAsync("SELECT Id FROM AppWriteLock WHERE Id=1 FOR UPDATE");
        var need = model.Id == 0 ? new ResourceNeed
        {
            public_actor_account_id = AccountId,
            Location = new Location()
        }

        : await context.ResourceNeeds.Include(need => need.Location).SingleOrDefaultAsync(need => need.resource_need_id == model.Id && need.public_actor_account_id == AccountId && !need.archived);
        if (need == null)
        {
            return NotFound();
        }

        if (model.Id != 0 && await context.Reservations.AnyAsync(reservation => reservation.ResourceMatch.resource_need_id == model.Id && (reservation.reservation_status == "Venter på svar" || reservation.reservation_status == "Godtatt") && (reservation.requested_start < model.Start || reservation.requested_end > model.End || reservation.ResourceMatch.Resource.resource_type_id != model.ResourceTypeId)))
        {
            ModelState.AddModelError(string.Empty, "Avklar aktive forespørsler før du endrer kategori eller innskrenker behovets periode.");
            model.Types = await context.ResourceTypes.AsNoTracking().ToListAsync();
            return View("Form", model);
        }

        need.title = model.Title.Trim();
        need.description = model.Description ?? string.Empty;
        need.priority = model.Priority;
        need.resource_type_id = model.ResourceTypeId!.Value;
        need.required_start = model.Start;
        need.required_end = model.End;
        need.deadline = model.Deadline;
        need.Location.address_or_area = model.Address.Trim();
        need.Location.latitude = (decimal)model.Latitude!.Value;
        need.Location.longitude = (decimal)model.Longitude!.Value;
        if (model.Id == 0)
        {
            context.ResourceNeeds.Add(need);
        }

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        TempData["Message"] = "Behovet er lagret.";
        return RedirectToAction(nameof(Details), new { id = need.resource_need_id });
    }

    [Authorize(Roles = "Public"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Status(int id, string status)
    {
        TempData["Message"] = await workflow.NeedStatusAsync(id, AccountId, status) ?? "Statusen er oppdatert.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Public"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(int id, bool restore = false)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        await context.Database.ExecuteSqlRawAsync("SELECT Id FROM AppWriteLock WHERE Id=1 FOR UPDATE");
        var need = await context.ResourceNeeds.SingleOrDefaultAsync(need => need.resource_need_id == id && need.public_actor_account_id == AccountId);
        if (need == null)
        {
            return NotFound();
        }

        if (!restore && (await context.Reservations.AnyAsync(reservation => reservation.ResourceMatch.resource_need_id == id && (reservation.reservation_status == "Godtatt" || reservation.reservation_status == "Venter på svar")) || await context.VolunteerInterests.AnyAsync(interest => interest.resource_need_id == id && (interest.interest_status == "Godtatt" || interest.interest_status == "Venter på svar"))))
        {
            TempData["Message"] = "Avklar aktive forespørsler og interesser før arkivering.";
            return RedirectToAction(nameof(Details), new { id });
        }

        need.archived = !restore;
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return RedirectToAction(nameof(Index), new { archived = !restore });
    }
}
