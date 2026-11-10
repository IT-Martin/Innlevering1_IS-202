using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reachout.DataAccess;
using Reachout.Models.Entities;

namespace Reachout.Services;

public static class InitialAdmin
{
    public static async Task InitializeAsync(ApplicationDbContext db, IConfiguration configuration, IHostEnvironment environment)
    {
        // Den delte demonstrasjonskontoen skal bare opprettes i utviklingsmiljøet.
        if (!environment.IsDevelopment())
        {
            return;
        }

        var email = configuration["InitialAdmin:Email"]?.Trim().ToLowerInvariant();
        var password = configuration["InitialAdmin:Password"];
        if (email is null && password is null)
        {
            return;
        }

        await using var transaction = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlRawAsync("SELECT Id FROM AppWriteLock WHERE Id = 1 FOR UPDATE");

        if (await db.Accounts.AnyAsync(account => account.account_type == "Platform"))
        {
            return;
        }

        if (string.IsNullOrEmpty(email) || email.Length > 255 || !new EmailAddressAttribute().IsValid(email)
            || password is null || password.Length is < 14 or > 128)
        {
            throw new InvalidOperationException("Oppgi gyldig lokal systemansvarligadresse og et passord på 14 til 128 tegn.");
        }

        if (await db.Accounts.AnyAsync(account => account.email == email))
        {
            throw new InvalidOperationException("Adressen for lokal systemansvarlig er allerede i bruk. Velg en annen.");
        }

        var administrator = new Account
        {
            email = email,
            contact_name = "Systemansvarlig",
            account_type = "Platform",
            email_confirmed = true
        };
        administrator.password_hash = new PasswordHasher<Account>().HashPassword(administrator, password);
        db.Accounts.Add(administrator);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
