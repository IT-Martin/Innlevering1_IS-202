using System.Security.Claims;
using Reachout.DataAccess;
using Reachout.Models;
using Reachout.Models.Entities;
using Reachout.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Controllers;
[EnableRateLimiting("accounts")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class LoginController : Controller
{
    // Gir tilgang til databasen slik at vi kan
    // hente, opprette og oppdatere brukerkontoer.
    private readonly ApplicationDbContext context;
    private readonly AccountService accounts;
    private readonly PasswordHasher<Account> passwordHasher = new();
    public LoginController(ApplicationDbContext context, AccountService accounts)
    {
        this.context = context;
        this.accounts = accounts;
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim().ToLowerInvariant();
        var account = await context.Accounts.SingleOrDefaultAsync(account => account.email == email);
        if (account == null || account.locked_until > DateTime.UtcNow)
        {
            ModelState.AddModelError(string.Empty, "Kunne ikke logge inn. Kontroller e-post og passord, eller prøv igjen om noen minutter.");
            return View(model);
        }

        var result = passwordHasher.VerifyHashedPassword(account, account.password_hash, model.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            account.failed_attempts++;
            if (account.failed_attempts >= 5)
            {
                account.locked_until = DateTime.UtcNow.AddMinutes(5);
                account.failed_attempts = 0;
            }

            await context.SaveChangesAsync();
            ModelState.AddModelError(string.Empty, "Feil e-post eller passord.");
            return View(model);
        }

        if (!await accounts.EligibleAsync(account))
        {
            ModelState.AddModelError(string.Empty, "Tilgangen er ikke aktiv. Offentlig konto trenger godkjent organisasjon og bekreftet jobbadresse.");
            return View(model);
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            account.password_hash = passwordHasher.HashPassword(account, model.Password);
        }

        account.failed_attempts = 0;
        account.locked_until = null;
        await context.SaveChangesAsync();
        await accounts.SignInAsync(HttpContext, account);
        return RedirectToAction(account.account_type == "Platform" ? "Index" : "Overview", account.account_type == "Platform" ? "Organizations" : "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Register(string? invitation = null)
    {
        var model = new AccountRegistrationViewModel
        {
            Invitation = invitation ?? string.Empty
        };
        if (!string.IsNullOrEmpty(invitation))
        {
            var hash = AccountService.Hash(invitation);
            var invite = await context.OrganizationInvitations.AsNoTracking().SingleOrDefaultAsync(invite => invite.token_hash == hash && !invite.revoked && invite.used_at == null && invite.expires_at > DateTime.UtcNow);
            if (invite != null)
            {
                model.Email = invite.email;
            }
        }

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(AccountRegistrationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim().ToLowerInvariant();
        if (await context.Accounts.AnyAsync(account => account.email == email))
        {
            ModelState.AddModelError(nameof(model.Email), "Det finnes allerede en konto med denne e-postadressen.");
            return View(model);
        }

        await using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        OrganizationInvitation? invite = null;
        if (!string.IsNullOrEmpty(model.Invitation))
        {
            var hash = AccountService.Hash(model.Invitation);
            invite = await context.OrganizationInvitations.Include(invite => invite.PublicOrganization).SingleOrDefaultAsync(invite => invite.token_hash == hash && invite.email == email && !invite.revoked && invite.used_at == null && invite.expires_at > DateTime.UtcNow && invite.PublicOrganization.approval_status == "Godkjent");
            if (invite == null)
            {
                ModelState.AddModelError(string.Empty, "Invitasjonen er ugyldig, brukt eller utløpt.");
                return View(model);
            }
        }

        if (model.ProviderType is not ("Privatperson" or "Bedrift" or "Organisasjon"))
        {
            ModelState.AddModelError(string.Empty, "Velg en gyldig leverandørtype.");
            return View(model);
        }

        var account = new Account
        {
            account_type = invite == null ? "Private" : "Public",
            email = email,
            contact_name = model.ContactName.Trim(),
            phone = model.Phone.Trim()
        };
        account.password_hash = passwordHasher.HashPassword(account, model.Password);
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        if (invite == null)
        {
            context.ProviderAccounts.Add(new ProviderAccount { account_id = account.account_id, contact_name = account.contact_name, phone = account.phone, provider_type = model.ProviderType, organization_name = model.Organization ?? string.Empty, address = string.Empty });
        }
        else
        {
            invite.used_at = DateTime.UtcNow;
            context.PublicActorAccounts.Add(new PublicActorAccount { account_id = account.account_id, public_organization_id = invite.public_organization_id, permission = invite.permission });
        }

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        if (invite != null)
        {
            try
            {
                var activation = await accounts.CreateTokenAsync(account, "EmailConfirmation");
                if (accounts.LocalPreview)
                {
                    TempData["LocalLink"] = activation.Value;
                }

                TempData["Message"] = "Kontoen er opprettet. Bekreft jobbadressen gjennom lenken før du logger inn.";
            }
            catch (Exception error)when (error is InvalidOperationException or System.Net.Mail.SmtpException)
            {
                TempData["Message"] = "Kontoen er opprettet uten aktiv tilgang. E-post kunne ikke sendes. Bruk «Send bekreftelse på nytt» når e-posttjenesten virker.";
            }

            return RedirectToAction(nameof(Index));
        }

        await accounts.SignInAsync(HttpContext, account);
        return RedirectToAction("Overview", "Home");
    }

    [HttpGet]
    public IActionResult Confirm(string token)
    {
        ViewBag.Token = token;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken, ActionName("Confirm")]
    public async Task<IActionResult> ConfirmPost(string token)
    {
        var hash = AccountService.Hash(token ?? string.Empty);
        var item = await context.AccountTokens.Include(item => item.Account).SingleOrDefaultAsync(item => item.token_hash == hash && item.purpose == "EmailConfirmation" && item.used_at == null && item.expires_at > DateTime.UtcNow && item.security_stamp == item.Account.security_stamp);
        if (item == null)
        {
            return BadRequest("Lenken er ugyldig, brukt eller utløpt.");
        }

        item.Account.email_confirmed = true;
        item.used_at = DateTime.UtcNow;
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest("Lenken er allerede brukt.");
        }

        TempData["Message"] = "Jobbadressen er bekreftet. Du kan logge inn.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendConfirmation(string? email)
    {
        var account = await context.Accounts.SingleOrDefaultAsync(account => account.email == (email ?? string.Empty).Trim().ToLowerInvariant() && !account.email_confirmed && account.account_type == "Public");
        if (account != null)
        {
            try
            {
                var result = await accounts.CreateTokenAsync(account, "EmailConfirmation");
                if (accounts.LocalPreview)
                {
                    TempData["LocalLink"] = result.Value;
                }
            }
            catch (Exception error)when (error is InvalidOperationException or System.Net.Mail.SmtpException)
            {
                TempData["Message"] = "E-posttjenesten er ikke tilgjengelig.";
                return RedirectToAction(nameof(Index));
            }
        }

        TempData["Message"] = "Hvis kontoen finnes og trenger bekreftelse, sendes en ny lenke.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();
}