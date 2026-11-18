using System.Security.Claims;
using Reachout.DataAccess;
using Reachout.Models;
using Reachout.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Controllers;
[Authorize(Roles = "Private,Public")]
public class VolunteersController : Controller
{
    private readonly ApplicationDbContext context;
    public VolunteersController(ApplicationDbContext context)
    {
        this.context = context;
    }

    private int AccountId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [Authorize(Roles = "Public")]
    public async Task<IActionResult> Index()
    {
        var profiles = await context.VolunteerProfiles.Include(profile => profile.ProviderAccount).ThenInclude(provider => provider.Account)
            .Include(profile => profile.VolunteerProfileSkills).ThenInclude(skill => skill.Skill)
            .AsNoTracking().Where(profile => !profile.archived).ToListAsync();
        var ids = profiles.Select(profile => profile.volunteer_profile_id).ToList();
        var periods = await context.AvailabilityPeriods.AsNoTracking()
            .Where(period => period.volunteer_profile_id.HasValue && ids.Contains(period.volunteer_profile_id.Value) && period.available_to > DateTime.Now)
            .OrderBy(period => period.available_from).ToListAsync();
        return View(profiles.Select(profile => new VolunteerDirectoryEntryViewModel
        {
            Profile = profile,
            Periods = periods.Where(period => period.volunteer_profile_id == profile.volunteer_profile_id).ToList()
        }).ToList());
    }

    [Authorize(Roles = "Private"), HttpGet]
    public async Task<IActionResult> Edit(int? needId)
    {
        var profile = await context.VolunteerProfiles.Include(profile => profile.VolunteerProfileSkills).ThenInclude(skill => skill.Skill).AsNoTracking().SingleOrDefaultAsync(profile => profile.account_id == AccountId);
        return View(profile == null ? new VolunteerProfileViewModel { NeedId = needId } : new VolunteerProfileViewModel { NeedId = needId, Id = profile.volunteer_profile_id, Area = profile.service_area, Role = profile.volunteer_role, Information = profile.additional_information, Skills = string.Join(", ", profile.VolunteerProfileSkills.Select(skill => skill.Skill.name)), Archived = profile.archived });
    }

    [Authorize(Roles = "Private"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(VolunteerProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await using var transaction = await context.Database.BeginTransactionAsync();
        await context.Database.ExecuteSqlRawAsync("SELECT Id FROM AppWriteLock WHERE Id=1 FOR UPDATE");
        var profile = await context.VolunteerProfiles.Include(profile => profile.VolunteerProfileSkills).SingleOrDefaultAsync(profile => profile.account_id == AccountId);
        if (profile == null)
        {
            profile = new VolunteerProfile
            {
                account_id = AccountId
            };
            context.VolunteerProfiles.Add(profile);
        }

        profile.service_area = model.Area.Trim();
        profile.volunteer_role = model.Role.Trim();
        profile.additional_information = model.Information ?? string.Empty;
        context.VolunteerProfileSkills.RemoveRange(profile.VolunteerProfileSkills);
        var skills = (model.Skills ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (skills.Count > 20 || skills.Any(skill => skill.Length > 200))
        {
            ModelState.AddModelError("Skills", "Bruk maks 20 ferdigheter på inntil 200 tegn hver.");
            return View(model);
        }

        await context.SaveChangesAsync();
        foreach (var name in skills)
        {
            var skill = await context.Skills.SingleOrDefaultAsync(skill => skill.name == name);
            if (skill == null)
            {
                skill = new Skill
                {
                    name = name
                };
                context.Skills.Add(skill);
            }

            context.VolunteerProfileSkills.Add(new VolunteerProfileSkill { volunteer_profile_id = profile.volunteer_profile_id, Skill = skill });
        }

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        TempData["Message"] = "Frivilligprofilen er lagret.";
        if (model.NeedId.HasValue)
        {
            return RedirectToAction("Details", "Needs", new { id = model.NeedId.Value });
        }

        return RedirectToAction(nameof(Edit));
    }

    [Authorize(Roles = "Private"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(bool restore = false, int? needId = null)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        await context.Database.ExecuteSqlRawAsync("SELECT Id FROM AppWriteLock WHERE Id=1 FOR UPDATE");
        var profile = await context.VolunteerProfiles.SingleOrDefaultAsync(profile => profile.account_id == AccountId);
        if (profile == null)
        {
            return NotFound();
        }

        if (!restore)
        {
            var activeInterests = await context.VolunteerInterests.Where(interest => interest.volunteer_profile_id == profile.volunteer_profile_id && (interest.interest_status == "Godtatt" || interest.interest_status == "Venter på svar")).ToListAsync();
            foreach (var interest in activeInterests)
            {
                interest.interest_status = "Trukket";
            }
        }

        profile.archived = !restore;
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        TempData["Message"] = restore ? "Profilen er gjenopprettet." : "Du er ikke lenger aktiv som frivillig. Aktive påmeldinger er trukket, og historikken er bevart.";
        if (restore && needId.HasValue)
        {
            return RedirectToAction("Details", "Needs", new { id = needId.Value });
        }
        return RedirectToAction(nameof(Edit));
    }
}
