using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reachout.DataAccess;
using Reachout.Models.Entities;

namespace Reachout.Services;

public static class InitialPublicActor
{
    public static async Task InitializeAsync(ApplicationDbContext db, IConfiguration configuration, IHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
        {
            return;
        }

        var email = configuration["InitialPublicActor:Email"]?.Trim().ToLowerInvariant();
        var password = configuration["InitialPublicActor:Password"];
        if (email is null && password is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(email) || email.Length > 255 || !new EmailAddressAttribute().IsValid(email)
            || password is null || password.Length is < 14 or > 128)
        {
            throw new InvalidOperationException("Oppgi gyldig lokal offentlig adresse og et passord på 14 til 128 tegn.");
        }

        await using var transaction = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlRawAsync("SELECT Id FROM AppWriteLock WHERE Id = 1 FOR UPDATE");

        var existing = await db.Accounts.SingleOrDefaultAsync(account => account.email == email);
        if (existing is not null)
        {
            if (existing.account_type != "Public" || !await db.PublicActorAccounts.AnyAsync(actor => actor.account_id == existing.account_id))
            {
                throw new InvalidOperationException("Adressen for lokal offentlig aktør er allerede i bruk av en annen konto.");
            }

            // Behold eksisterende passord, godkjenningsstatus og tilgang ved omstart.
            return;
        }

        const string organizationNumber = "DEMO-KRS";
        var organization = await db.PublicOrganizations.SingleOrDefaultAsync(org => org.organization_number == organizationNumber);
        if (organization is not null)
        {
            throw new InvalidOperationException("Demonstrasjonsorganisasjonen finnes allerede. Opprett flere medlemmer med invitasjon.");
        }

        organization = new PublicOrganization
        {
            organization_name = "Kristiansand kommune (demo)",
            organization_type = "Kommune",
            organization_number = organizationNumber,
            email_domain = email[(email.LastIndexOf('@') + 1)..],
            contact_email = email,
            contact_name = "Offentlig demonstrasjonsbruker",
            approval_status = "Godkjent",
            verification = "Lokal demonstrasjonsorganisasjon, ingen faktisk verifisering av kommunen."
        };
        var account = new Account
        {
            email = email,
            contact_name = "Kristiansand kommune (demo)",
            account_type = "Public",
            email_confirmed = true
        };
        account.password_hash = new PasswordHasher<Account>().HashPassword(account, password);
        db.PublicActorAccounts.Add(new PublicActorAccount
        {
            Account = account,
            PublicOrganization = organization,
            permission = "Organisasjonsadministrator"
        });
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
