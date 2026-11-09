using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Reachout.DataAccess;
using Reachout.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Services;
public class AccountService
{
    private readonly ApplicationDbContext context;
    private readonly IConfiguration configuration;
    public AccountService(ApplicationDbContext context, IConfiguration configuration)
    {
        this.context = context;
        this.configuration = configuration;
    }

    public bool LocalPreview => configuration.GetValue<bool>("LocalPreview");

    public static string Token() => Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    public static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    public async Task<bool> EligibleAsync(Account account)
    {
        if (account.access_revoked)
        {
            return false;
        }

        if (account.account_type != "Public")
        {
            return true;
        }

        return account.email_confirmed && await context.PublicActorAccounts.AnyAsync(actor => actor.account_id == account.account_id && actor.PublicOrganization.approval_status == "Godkjent");
    }

    public async Task<(AccountToken Token, string Value)> CreateTokenAsync(Account account, string purpose)
    {
        if (purpose is not ("PasswordReset" or "EmailConfirmation"))
        {
            throw new ArgumentException("Ugyldig formål for lenken.", nameof(purpose));
        }

        var code = Token();
        var token = new AccountToken
        {
            account_id = account.account_id,
            purpose = purpose,
            token_hash = Hash(code),
            security_stamp = account.security_stamp,
            expires_at = DateTime.UtcNow.AddMinutes(15)
        };
        context.AccountTokens.Add(token);
        await context.SaveChangesAsync();
        var path = (purpose == "PasswordReset" ? "/Account/NewPassword?token=" : "/Login/Confirm?token=") + code;
        await SendAsync(account.email, "Åpne den personlige lenken: " + path, path);
        return (token, path);
    }

    public async Task<string?> SendAsync(string email, string text, string localValue)
    {
        if (LocalPreview)
        {
            return localValue;
        }

        var host = configuration["Smtp:Host"];
        var origin = configuration["PublicBaseUrl"];
        if (string.IsNullOrWhiteSpace(host) || !Uri.TryCreate(origin, UriKind.Absolute, out var address) || address.Scheme != "https")
        {
            throw new InvalidOperationException("SMTP og en fast HTTPS-adresse må konfigureres.");
        }

        if (localValue.StartsWith('/'))
        {
            text = "Åpne " + origin!.TrimEnd('/') + localValue;
        }

        using var smtp = new SmtpClient(host, configuration.GetValue<int?>("Smtp:Port") ?? 587)
        {
            EnableSsl = true
        };
        if (!string.IsNullOrEmpty(configuration["Smtp:User"]))
        {
            smtp.Credentials = new NetworkCredential(configuration["Smtp:User"], configuration["Smtp:Password"]);
        }

        using var mail = new MailMessage(configuration["Smtp:From"] ?? throw new InvalidOperationException("Avsender mangler."), email, "Reachout", text);
        await smtp.SendMailAsync(mail);
        return null;
    }

    public async Task SignInAsync(HttpContext httpContext, Account account)
    {
        if (!await EligibleAsync(account))
        {
            throw new InvalidOperationException("Kontoen har ikke aktiv tilgang.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.account_id.ToString()),
            new(ClaimTypes.Email, account.email),
            new(ClaimTypes.Name, account.contact_name),
            new(ClaimTypes.Role, account.account_type),
            new("SecurityStamp", account.security_stamp)
        };
        var actor = await context.PublicActorAccounts.AsNoTracking().SingleOrDefaultAsync(actor => actor.account_id == account.account_id);
        if (actor != null)
        {
            claims.Add(new Claim("Permission", actor.permission));
            claims.Add(new Claim("Organization", actor.public_organization_id.ToString()));
        }

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
    }
}
