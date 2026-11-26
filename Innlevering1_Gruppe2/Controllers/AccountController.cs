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
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class AccountController : Controller
{
    private readonly ApplicationDbContext context;
    private readonly AccountService accounts;
    public AccountController(ApplicationDbContext context, AccountService accounts)
    {
        this.context = context;
        this.accounts = accounts;
    }

    private int AccountId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [Authorize, HttpGet]
    public async Task<IActionResult> Profile()
    {
        var account = await context.Accounts.AsNoTracking().SingleAsync(account => account.account_id == AccountId);
        var provider = await context.ProviderAccounts.AsNoTracking().SingleOrDefaultAsync(provider => provider.account_id == AccountId);
        return View(new ProfileViewModel { Name = account.contact_name, Phone = account.phone, Email = account.email, Address = provider?.address ?? string.Empty, Organization = provider?.organization_name ?? string.Empty, OrganizationNumber = provider?.organization_number ?? string.Empty, ProviderType = provider?.provider_type ?? "Privatperson" });
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        var account = await context.Accounts.SingleAsync(account => account.account_id == AccountId);
        model.Email = account.email;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var provider = await context.ProviderAccounts.SingleOrDefaultAsync(provider => provider.account_id == AccountId);
        if (provider != null)
        {
            if (model.ProviderType is not ("Privatperson" or "Bedrift" or "Organisasjon"))
            {
                return BadRequest();
            }

            provider.provider_type = model.ProviderType;
            provider.contact_name = model.Name.Trim();
            provider.phone = model.Phone.Trim();
            provider.address = model.Address ?? string.Empty;
            provider.organization_name = model.Organization ?? string.Empty;
            provider.organization_number = model.OrganizationNumber ?? string.Empty;
        }

        account.contact_name = model.Name.Trim();
        account.phone = model.Phone.Trim();
        await context.SaveChangesAsync();
        TempData["Message"] = "Profilen er lagret.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Password(string current, string password, string confirm)
    {
        var account = await context.Accounts.SingleAsync(account => account.account_id == AccountId);
        var hasher = new PasswordHasher<Account>();
        if (string.IsNullOrEmpty(current) || string.IsNullOrEmpty(password) || password.Length < (account.account_type == "Platform" ? 14 : 8) || password.Length > 128 || password != confirm || hasher.VerifyHashedPassword(account, account.password_hash, current) == PasswordVerificationResult.Failed)
        {
            TempData["Message"] = "Kontroller gjeldende passord. Nye passord må være like og ha minst 8 tegn (14 for systemansvarlig).";
            return RedirectToAction(nameof(Profile));
        }

        account.password_hash = hasher.HashPassword(account, password);
        account.security_stamp = Guid.NewGuid().ToString();
        await context.SaveChangesAsync();
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Login");
    }

    [HttpGet]
    public IActionResult Forgot() => View();
    [HttpPost, ValidateAntiForgeryToken, EnableRateLimiting("accounts")]
    public async Task<IActionResult> Forgot(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(string.Empty, "Oppgi e-postadressen.");
            return View();
        }

        var account = await context.Accounts.SingleOrDefaultAsync(account => account.email == email.Trim().ToLowerInvariant());
        if (account != null)
        {
            try
            {
                var token = await accounts.CreateTokenAsync(account, "PasswordReset");
                if (accounts.LocalPreview)
                {
                    TempData["LocalLink"] = token.Value;
                }
            }
            catch (Exception error)when (error is InvalidOperationException or System.Net.Mail.SmtpException)
            {
                ModelState.AddModelError(string.Empty, "E-posttjenesten er ikke tilgjengelig. Prøv igjen senere.");
                return View();
            }
        }

        TempData["Message"] = "Hvis kontoen finnes, er det sendt en lenke som varer i 15 minutter.";
        return RedirectToAction("Index", "Login");
    }

    [HttpGet]
    public IActionResult NewPassword(string token)
    {
        ViewBag.Token = token;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken, EnableRateLimiting("accounts")]
    public async Task<IActionResult> NewPassword(string token, string password, string confirm)
    {
        ViewBag.Token = token;
        var hash = AccountService.Hash(token ?? string.Empty);
        var link = await context.AccountTokens.Include(link => link.Account).SingleOrDefaultAsync(link => link.token_hash == hash && link.purpose == "PasswordReset" && link.used_at == null && link.expires_at > DateTime.UtcNow && link.security_stamp == link.Account.security_stamp);
        if (link == null)
        {
            ModelState.AddModelError(string.Empty, "Lenken er ugyldig, brukt eller utløpt.");
            return View();
        }

        if (string.IsNullOrEmpty(password) || password.Length < (link.Account.account_type == "Platform" ? 14 : 8) || password.Length > 128 || password != confirm)
        {
            ModelState.AddModelError(string.Empty, "Skriv like passord på minst 8 tegn (14 for systemansvarlig).");
            return View();
        }

        link.Account.password_hash = new PasswordHasher<Account>().HashPassword(link.Account, password);
        link.Account.security_stamp = Guid.NewGuid().ToString();
        link.used_at = DateTime.UtcNow;
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(string.Empty, "Lenken er allerede brukt.");
            return View();
        }

        await HttpContext.SignOutAsync();
        TempData["Message"] = "Passordet er endret. Logg inn med det nye passordet.";
        return RedirectToAction("Index", "Login");
    }
}