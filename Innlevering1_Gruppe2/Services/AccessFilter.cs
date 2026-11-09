using System.Security.Claims;
using Reachout.DataAccess;
using Reachout.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Services;
public class AccessFilter : IAsyncActionFilter
{
    private readonly ApplicationDbContext context;
    private readonly AccountService accounts;
    private readonly IServiceScopeFactory scopeFactory;
    public AccessFilter(ApplicationDbContext context, AccountService accounts, IServiceScopeFactory scopeFactory)
    {
        this.context = context;
        this.accounts = accounts;
        this.scopeFactory = scopeFactory;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext filter, ActionExecutionDelegate next)
    {
        var user = filter.HttpContext.User;
        var controller = filter.RouteData.Values["controller"]?.ToString();
        var action = filter.RouteData.Values["action"]?.ToString();
        var post = HttpMethods.IsPost(filter.HttpContext.Request.Method);
        if (user.Identity?.IsAuthenticated == true)
        {
            filter.HttpContext.Response.Headers.CacheControl = "no-store";
            int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id);
            var account = await context.Accounts.AsNoTracking().SingleOrDefaultAsync(account => account.account_id == id);
            var actor = await context.PublicActorAccounts.AsNoTracking().SingleOrDefaultAsync(actor => actor.account_id == id);
            if (account == null || account.security_stamp != user.FindFirstValue("SecurityStamp") || !await accounts.EligibleAsync(account) || !user.IsInRole(account.account_type) || actor != null && actor.permission != user.FindFirstValue("Permission"))
            {
                await filter.HttpContext.SignOutAsync();
                filter.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

            var readOnly = user.IsInRole("Public") && actor?.permission == "Lesetilgang";
            if (readOnly && ((post && (controller is "Needs" or "Requests" or "Interests")) || controller == "Exports" || controller == "Needs" && (action is "Create" or "Edit")))
            {
                filter.Result = new ForbidResult();
                return;
            }
        }

        var result = await next();
        if (post && result.Exception == null && user.Identity?.IsAuthenticated == true && result.Result is not ForbidResult)
        {
            int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id);
            // Loggfør med egen kontekst, slik at ugyldige skjemaverdier ikke lagres.
            await using var scope = scopeFactory.CreateAsyncScope();
            var auditContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            auditContext.AuditEntries.Add(new AuditEntry { account_id = id, action = controller + "." + action, target = filter.RouteData.Values["id"]?.ToString() ?? string.Empty });
            await auditContext.SaveChangesAsync();
        }
    }
}
