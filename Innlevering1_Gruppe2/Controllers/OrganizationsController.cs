using System.Security.Claims;
using Reachout.DataAccess;
using Reachout.Models;
using Reachout.Models.Entities;
using Reachout.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Controllers;
[EnableRateLimiting("accounts")]
public class OrganizationsController : Controller
{
    private readonly ApplicationDbContext context;
    private readonly AccountService accounts;
    public OrganizationsController(ApplicationDbContext context, AccountService accounts)
    {
        this.context = context;
        this.accounts = accounts;
    }

    private bool Platform => User.IsInRole("Platform");
    private bool Administrator => Platform || User.IsInRole("Public") && User.FindFirstValue("Permission") == "Organisasjonsadministrator";
    private int AccountId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private int OrganizationId => int.TryParse(User.FindFirstValue("Organization"), out var id) ? id : 0;

    [AllowAnonymous, HttpGet]
    public IActionResult Apply() => View(new OrganizationFormViewModel());
    [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(OrganizationFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim().ToLowerInvariant();
        var domain = email[(email.LastIndexOf('@') + 1)..];
        if (domain.Length > 200 || !System.Text.RegularExpressions.Regex.IsMatch(domain, @"^[a-z0-9](?:[a-z0-9.-]*[a-z0-9])?\.[a-z]{2,}$"))
        {
            ModelState.AddModelError(nameof(model.Email), "Oppgi en jobb-e-post med et gyldig domene.");
        }

        var number = model.NormalizedNumber;
        if (await context.PublicOrganizations.AnyAsync(org => org.organization_number == number))
        {
            ModelState.AddModelError("Number", "Organisasjonen har allerede en søknad. Kontakt systemansvarlig.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        context.PublicOrganizations.Add(new PublicOrganization { organization_name = model.Name.Trim(), organization_type = model.Type.Trim(), organization_number = number, email_domain = domain, contact_email = email, contact_name = model.Contact.Trim(), phone = model.Phone ?? string.Empty });
        await context.SaveChangesAsync();
        TempData["OrganizationApplicationReceived"] = true;
        return RedirectToAction(nameof(ApplicationReceived));
    }

    [AllowAnonymous, HttpGet]
    public IActionResult ApplicationReceived()
    {
        return TempData["OrganizationApplicationReceived"] is true
            ? View()
            : RedirectToAction(nameof(Apply));
    }

    [Authorize(Roles = "Platform,Public"), HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!Administrator)
        {
            return Forbid();
        }

        var organizations = await context.PublicOrganizations.AsNoTracking().Where(org => Platform || org.public_organization_id == OrganizationId).ToListAsync();
        var members = await context.PublicActorAccounts.Include(actor => actor.Account).Include(actor => actor.PublicOrganization).AsNoTracking().Where(actor => Platform || actor.public_organization_id == OrganizationId).ToListAsync();
        var ids = members.Select(member => member.account_id).Append(AccountId).ToList();
        return View(new OrganizationAdminViewModel { Platform = Platform, Organizations = organizations, Members = members, Invitations = await context.OrganizationInvitations.AsNoTracking().Where(invite => Platform || invite.public_organization_id == OrganizationId).OrderByDescending(invite => invite.invitation_id).ToListAsync(), Audit = await context.AuditEntries.AsNoTracking().Where(entry => Platform || entry.account_id.HasValue && ids.Contains(entry.account_id.Value)).OrderByDescending(entry => entry.occurred_at).Take(100).ToListAsync() });
    }

    [Authorize(Roles = "Platform"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Review(int id, string status, string evidence)
    {
        if (status is not ("Godkjent" or "Sperret" or "Avslått") || string.IsNullOrWhiteSpace(evidence) || evidence.Length < 15 || evidence.Length > 1000)
        {
            return BadRequest();
        }

        var organization = await context.PublicOrganizations.SingleOrDefaultAsync(org => org.public_organization_id == id);
        if (organization == null)
        {
            return NotFound();
        }

        organization.approval_status = status;
        organization.verification = evidence.Trim();
        await context.SaveChangesAsync();
        TempData["Message"] = "Organisasjonens status er oppdatert. Ved sperring mister ansatte tilgangen ved neste forespørsel.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Platform,Public"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Invite(int id, string email, string permission)
    {
        if (!Administrator || !Platform && id != OrganizationId)
        {
            return Forbid();
        }

        if (permission is not ("Beredskapsansatt" or "Lesetilgang" or "Organisasjonsadministrator") || string.IsNullOrWhiteSpace(email) || email.Length > 255)
        {
            return BadRequest();
        }

        email = email.Trim().ToLowerInvariant();
        var organization = await context.PublicOrganizations.AsNoTracking().SingleOrDefaultAsync(org => org.public_organization_id == id && org.approval_status == "Godkjent");
        if (organization == null || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email) || !email.EndsWith("@" + organization.email_domain, StringComparison.OrdinalIgnoreCase) || await context.Accounts.AnyAsync(account => account.email == email))
        {
            TempData["Message"] = "Velg en godkjent organisasjon og en ledig jobbadresse på riktig domene.";
            return RedirectToAction(nameof(Index));
        }

        var token = AccountService.Token();
        var invitation = new OrganizationInvitation
        {
            public_organization_id = id,
            email = email,
            permission = permission,
            token_hash = AccountService.Hash(token)
        };
        context.OrganizationInvitations.Add(invitation);
        await context.SaveChangesAsync();
        try
        {
            var local = await accounts.SendAsync(email, "Du er invitert til Beredskapssentralen.", "/Login/Register?invitation=" + token);
            if (local != null)
            {
                TempData["LocalLink"] = local;
            }

            TempData["Message"] = local == null ? "Invitasjonen er sendt og varer i to døgn." : "Lokal testinvitasjon opprettet. Ingen e-post er sendt.";
        }
        catch (Exception error)when (error is InvalidOperationException or System.Net.Mail.SmtpException)
        {
            invitation.revoked = true;
            await context.SaveChangesAsync();
            TempData["Message"] = "Invitasjonen ble sperret fordi e-posttjenesten ikke kunne sende.";
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Platform,Public"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RevokeInvitation(int id)
    {
        if (!Administrator)
        {
            return Forbid();
        }

        var invite = await context.OrganizationInvitations.SingleOrDefaultAsync(invite => invite.invitation_id == id && (Platform || invite.public_organization_id == OrganizationId));
        if (invite == null)
        {
            return NotFound();
        }

        invite.revoked = true;
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Platform,Public"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Member(int id, string permission, bool revoked)
    {
        if (!Administrator)
        {
            return Forbid();
        }

        if (permission is not ("Beredskapsansatt" or "Lesetilgang" or "Organisasjonsadministrator"))
        {
            return BadRequest();
        }

        await using var transaction = await context.Database.BeginTransactionAsync();
        await context.Database.ExecuteSqlRawAsync("SELECT Id FROM AppWriteLock WHERE Id=1 FOR UPDATE");
        var member = await context.PublicActorAccounts.Include(actor => actor.Account).SingleOrDefaultAsync(actor => actor.account_id == id && actor.account_id != AccountId && (Platform || actor.public_organization_id == OrganizationId));
        if (member == null)
        {
            return NotFound();
        }

        if (member.permission == "Organisasjonsadministrator" && !member.Account.access_revoked && (revoked || permission != "Organisasjonsadministrator") && !await context.PublicActorAccounts.AnyAsync(other => other.public_organization_id == member.public_organization_id && other.account_id != id && other.permission == "Organisasjonsadministrator" && !other.Account.access_revoked))
        {
            TempData["Message"] = "Behold minst én aktiv organisasjonsadministrator.";
            return RedirectToAction(nameof(Index));
        }

        member.permission = permission;
        member.Account.access_revoked = revoked;
        member.Account.security_stamp = Guid.NewGuid().ToString();
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        TempData["Message"] = "Ansattilgangen er oppdatert. Tidligere innlogginger er ugyldige.";
        return RedirectToAction(nameof(Index));
    }
}
