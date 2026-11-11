using System.Security.Claims;
using Reachout.DataAccess;
using Reachout.Models;
using Reachout.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Controllers;
[Authorize(Roles = "Private,Public")]
public class RequestsController : Controller
{
    private readonly ApplicationDbContext context;
    private readonly WorkflowService workflow;
    public RequestsController(ApplicationDbContext context, WorkflowService workflow)
    {
        this.context = context;
        this.workflow = workflow;
    }

    private int AccountId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(string status = "Venter på svar")
    {
        ViewBag.Status = status;
        var supplier = User.IsInRole("Private");
        var query = context.Reservations.Include(reservation => reservation.ResourceMatch).ThenInclude(match => match.Resource).Include(reservation => reservation.ResourceMatch.ResourceNeed).AsNoTracking().Where(reservation => supplier ? reservation.ResourceMatch.Resource.account_id == AccountId : reservation.public_actor_account_id == AccountId);
        if (status == "Avsluttet")
        {
            query = query.Where(reservation => reservation.reservation_status != "Venter på svar" && reservation.reservation_status != "Godtatt");
        }
        else if (status != "Alle")
        {
            query = query.Where(reservation => reservation.reservation_status == status);
        }

        return View(await query.OrderByDescending(reservation => reservation.reservation_id).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Respond(int id, string status)
    {
        TempData["Message"] = await workflow.RespondAsync(id, AccountId, User.IsInRole("Private"), status) ?? "Forespørselen er oppdatert.";
        return RedirectToAction(nameof(Index), new { status = "Alle" });
    }
}
