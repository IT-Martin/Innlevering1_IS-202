using System.Security.Claims;
using Reachout.DataAccess;
using Reachout.Models;
using Reachout.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Controllers;
[Authorize(Roles = "Private")]
public class AvailabilityController : Controller
{
    private readonly ApplicationDbContext context;
    private readonly WorkflowService workflow;
    public AvailabilityController(ApplicationDbContext context, WorkflowService workflow)
    {
        this.context = context;
        this.workflow = workflow;
    }

    private int AccountId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(int id, bool volunteer = false)
    {
        var contribution = volunteer
            ? await context.VolunteerProfiles.Where(profile => profile.volunteer_profile_id == id && profile.account_id == AccountId)
                .Select(profile => new { Name = profile.volunteer_role, Archived = profile.archived }).FirstOrDefaultAsync()
            : await context.Resources.Where(resource => resource.resource_id == id && resource.account_id == AccountId)
                .Select(resource => new { Name = resource.resource_name, Archived = resource.archived }).FirstOrDefaultAsync();
        if (contribution == null)
        {
            return NotFound();
        }

        var periods = await context.AvailabilityPeriods.AsNoTracking().Where(period => volunteer ? period.volunteer_profile_id == id : period.resource_id == id).OrderBy(period => period.available_from).ToListAsync();
        return View(new AvailabilityViewModel { Id = id, Name = contribution.Name, Archived = contribution.Archived, Volunteer = volunteer, Periods = periods });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int id, bool volunteer, DateTime start, DateTime end)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        TempData["Message"] = await workflow.AddPeriodAsync(id, AccountId, volunteer, start, end) ?? "Tilgjengelig periode er lagt til.";
        return RedirectToAction(nameof(Index), new { id, volunteer });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id, int ownerId, bool volunteer)
    {
        TempData["Message"] = await workflow.RemovePeriodAsync(id, AccountId) ?? "Perioden er fjernet.";
        return RedirectToAction(nameof(Index), new { id = ownerId, volunteer });
    }
}
