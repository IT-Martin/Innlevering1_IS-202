using System.Security.Claims;
using Reachout.DataAccess;
using Reachout.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Controllers;
[Authorize(Roles = "Private,Public")]
public class InterestsController : Controller
{
    private readonly ApplicationDbContext context;
    private readonly WorkflowService workflow;
    public InterestsController(ApplicationDbContext context, WorkflowService workflow)
    {
        this.context = context;
        this.workflow = workflow;
    }

    private int AccountId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(string status = "Venter på svar")
    {
        ViewBag.Status = status;
        var supplier = User.IsInRole("Private");
        var query = context.VolunteerInterests.Include(interest => interest.ResourceNeed).Include(interest => interest.VolunteerProfile).ThenInclude(profile => profile.ProviderAccount).ThenInclude(provider => provider.Account).AsNoTracking().Where(interest => supplier ? interest.VolunteerProfile.account_id == AccountId : interest.ResourceNeed.public_actor_account_id == AccountId);
        if (status == "Avsluttet")
        {
            query = query.Where(interest => interest.interest_status != "Venter på svar" && interest.interest_status != "Godtatt");
        }
        else if (status != "Alle")
        {
            query = query.Where(interest => interest.interest_status == status);
        }

        return View(await query.OrderByDescending(interest => interest.submitted_at).ToListAsync());
    }

    [Authorize(Roles = "Private"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int id, string message)
    {
        if (string.IsNullOrWhiteSpace(message) || message.Length > 2000)
        {
            return BadRequest();
        }

        TempData["Message"] = await workflow.InterestAsync(id, AccountId, message.Trim()) ?? "Påmeldingen er sendt. Du får svar her når kontaktpersonen har vurdert bidraget ditt.";
        return RedirectToAction("Details", "Needs", new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Respond(int id, string status)
    {
        TempData["Message"] = await workflow.InterestStatusAsync(id, AccountId, User.IsInRole("Private"), status) ?? "Påmeldingen er oppdatert.";
        return RedirectToAction(nameof(Index), new { status = "Alle" });
    }
}
