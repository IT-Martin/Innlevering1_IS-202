using Innlevering1_Gruppe2.Models;
using Innlevering1_Gruppe2.DataAccess;
using Innlevering1_Gruppe2.DataAccess.Repositories;
using Innlevering1_Gruppe2.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;

namespace Innlevering1_Gruppe2.Controllers;

public class HomeController : Controller
{
    private readonly IResourceRepository resourceRepository;
    private readonly ApplicationDbContext context;

    public HomeController(
        IResourceRepository resourceRepository,
        ApplicationDbContext context)
    {
        this.resourceRepository = resourceRepository;
        this.context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    //Kart med GET Request, sender ressurslisten til kartvisning

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Map()
    {
        var accountId = GetCurrentAccountId();
        if (accountId is null)
        {
            return Challenge();
        }

        var resources = await resourceRepository.GetByAccountIdAsync(accountId.Value);
        var viewModels = resources.Select(resource => new RessursViewModel
        {
            Id = resource.resource_id,
            RessursNavn = resource.resource_name,
            TypeIcon = ResourceTypeIcon.For(
                resource.ResourceType.category_name,
                resource.resource_name),
            Latitude = (double)resource.Location.latitude,
            Longitude = (double)resource.Location.longitude
        }).ToList();

        return View(viewModels);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Register()
    {
        await LoadRegistrationOptionsAsync();
        return View();
    }

    //Brukern registrere data med ressurs navn, og koordinater som er valgt på kartet.
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        string RessursNavn,
        string? Description,
        int? resourceTypeId,
        double? latitude,
        double? longitude)
    {
        if (string.IsNullOrWhiteSpace(RessursNavn))
        {
            ViewData["ErrorMessage"] =
                "Skriv inn navnet på ressursen.";
            await LoadRegistrationOptionsAsync();
            return View();
        }

        if (latitude is null || longitude is null)
        {
            ViewData["ErrorMessage"] =
                "Velg en posisjon på kartet.";
            await LoadRegistrationOptionsAsync();
            return View();
        }

        if (resourceTypeId is null)
        {
            ViewData["ErrorMessage"] =
                "Velg type ressurs.";
            await LoadRegistrationOptionsAsync();
            return View();
        }

        var validResourceType = await context.ResourceTypes
            .AnyAsync(type => type.resource_type_id == resourceTypeId.Value);

        if (!validResourceType)
        {
            ViewData["ErrorMessage"] =
                "Ressurstypen finnes ikke.";
            await LoadRegistrationOptionsAsync();
            return View();
        }

        var accountIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(accountIdValue, out var accountId))
        {
            return Challenge();
        }

        var account = await context.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(account => account.account_id == accountId);

        if (account is null)
        {
            return Challenge();
        }

        var coordinateText = string.Format(
            CultureInfo.InvariantCulture,
            "Koordinater: {0:F6}, {1:F6}",
            latitude.Value,
            longitude.Value);

        var resource = new Resource
        {
            account_id = accountId,
            resource_name = RessursNavn,
            description = Description ?? string.Empty,
            contact_details = $"Navn: {account.contact_name}, Telefon: {account.phone}",
            verification_status = "Pending",

            resource_type_id = resourceTypeId.Value,
            Location = new Location
            {
                address_or_area = coordinateText,
                latitude = (decimal)latitude.Value,
                longitude = (decimal)longitude.Value
            }
        };

        await resourceRepository.AddAsync(resource);

        return RedirectToAction(
            nameof(Result),
            new { id = resource.resource_id });

    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> MyResources()
    {
        var accountId = GetCurrentAccountId();
        if (accountId is null)
        {
            return Challenge();
        }

        var resources = await resourceRepository.GetByAccountIdAsync(accountId.Value);
        return View(resources);
    }

    [Authorize]
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
        return View(new ResourceEditViewModel
        {
            Id = resource.resource_id,
            Name = resource.resource_name,
            Description = resource.description,
            ResourceTypeId = resource.resource_type_id,
            Latitude = (double)resource.Location.latitude,
            Longitude = (double)resource.Location.longitude
        });
    }

    [Authorize]
    [HttpPost]
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

        var validResourceType = await context.ResourceTypes
            .AnyAsync(type => type.resource_type_id == model.ResourceTypeId!.Value);

        if (!validResourceType)
        {
            ModelState.AddModelError(
                nameof(model.ResourceTypeId),
                "Ressurstypen finnes ikke.");
            await LoadRegistrationOptionsAsync();
            return View(model);
        }

        var resource = await resourceRepository.GetByIdForAccountAsync(model.Id, accountId.Value);
        if (resource is null)
        {
            return NotFound();
        }

        var account = await context.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(account => account.account_id == accountId.Value);

        if (account is null)
        {
            return Challenge();
        }

        resource.resource_name = model.Name.Trim();
        resource.description = model.Description?.Trim() ?? string.Empty;
        resource.resource_type_id = model.ResourceTypeId.Value;
        resource.contact_details =
            $"Navn: {account.contact_name}, Telefon: {account.phone}";
        resource.Location.latitude = (decimal)model.Latitude!.Value;
        resource.Location.longitude = (decimal)model.Longitude!.Value;
        resource.Location.address_or_area = FormatCoordinates(
            model.Latitude.Value,
            model.Longitude.Value);

        await resourceRepository.UpdateAsync(resource);
        return RedirectToAction(nameof(MyResources));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var accountId = GetCurrentAccountId();
        if (accountId is null)
        {
            return Challenge();
        }

        await resourceRepository.DeleteAsync(id, accountId.Value);
        return RedirectToAction(nameof(MyResources));
    }

    private async Task LoadRegistrationOptionsAsync()
    {
        ViewBag.ResourceTypes = await context.ResourceTypes
            .AsNoTracking()
            .OrderBy(type => type.category_name)
            .ToListAsync();
    }

    private int? GetCurrentAccountId()
    {
        var accountIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(accountIdValue, out var accountId)
            ? accountId
            : null;
    }

    private static string FormatCoordinates(double latitude, double longitude)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "Koordinater: {0:F6}, {1:F6}",
            latitude,
            longitude);
    }

    //Finner ressursen med den oppgitte ID-en og viser den på resultatsiden.
    //Hvis ressursen ikke finnes i databasen, videresendes brukeren til kartet.
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Result(int? id)
    {
        if (id is null)
        {
            return RedirectToAction(nameof(Map));
        }

        var accountId = GetCurrentAccountId();
        if (accountId is null)
        {
            return Challenge();
        }

        var resource = await resourceRepository.GetByIdForAccountAsync(
            id.Value,
            accountId.Value);
        if (resource is null)
        {
            return RedirectToAction(nameof(Map));
        }

        return View(new RessursViewModel
        {
            Id = resource.resource_id,
            RessursNavn = resource.resource_name,
            Latitude = (double)resource.Location.latitude,
            Longitude = (double)resource.Location.longitude
        });
    }


    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}