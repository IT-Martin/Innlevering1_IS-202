using Innlevering1_Gruppe2.DataAccess;
using Innlevering1_Gruppe2.Models;
using Innlevering1_Gruppe2.Models.Entities;
using Innlevering1_Gruppe2.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Innlevering1_Gruppe2.Controllers;

public class LoginController : Controller
{
    private readonly ApplicationDbContext context;
    private readonly PasswordHasher<Account> passwordHasher = new();

    public LoginController(ApplicationDbContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim().ToLowerInvariant();
        var account = await context.Accounts
            .SingleOrDefaultAsync(a => a.email == email);

        if (account is null)
        {
            ModelState.AddModelError(string.Empty,
                "Feil e-post eller passord.");
            return View(model);
        }

        var passwordResult = passwordHasher.VerifyHashedPassword(
            account,
            account.password_hash,
            model.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty,
                "Feil e-post eller passord.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.account_id.ToString()),
            new(ClaimTypes.Email, account.email),
            new(ClaimTypes.Role, account.account_type)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return RedirectToAction("Map", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new AccountRegistrationViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(AccountRegistrationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim().ToLowerInvariant();
        var emailExists = await context.Accounts
            .AnyAsync(account => account.email == email);

        if (emailExists)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "Det finnes allerede en konto med denne e-postadressen.");
            return View(model);
        }

        var account = new Account
        {
            account_type = "Private",
            email = email,
            contact_name = model.ContactName.Trim(),
            phone = model.Phone.Trim()
        };

        account.password_hash = passwordHasher.HashPassword(
            account,
            model.Password);

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        await SignInAsync(account);
        return RedirectToAction("Map", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task SignInAsync(Account account)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.account_id.ToString()),
            new(ClaimTypes.Email, account.email),
            new(ClaimTypes.Role, account.account_type)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }
}