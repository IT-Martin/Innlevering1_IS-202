using Reachout.DataAccess;
using Reachout.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Services;
public static class DemoData
{
    public static async Task InitializeAsync(ApplicationDbContext context, IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("LocalPreview") || !configuration.GetValue<bool>("SeedDemoData"))
        {
            return;
        }

        async Task<PublicOrganization> Organization(string number, string name, string domain)
        {
            var organization = await context.PublicOrganizations.SingleOrDefaultAsync(org => org.organization_number == number);
            if (organization != null)
            {
                return organization;
            }

            organization = new PublicOrganization
            {
                organization_name = name,
                organization_number = number,
                email_domain = domain,
                contact_name = "Lokal testkontakt",
                contact_email = "kontakt@" + domain,
                approval_status = "Godkjent",
                verification = "Lokale eksempeldata. Ikke kontrollert offentlig organisasjon."
            };
            context.PublicOrganizations.Add(organization);
            await context.SaveChangesAsync();
            return organization;
        }

        var south = await Organization("111111111", "Testkommune Sør", "sor.example.test");
        var north = await Organization("222222222", "Testkommune Nord", "nord.example.test");
        async Task<Account> User(string email, string name, string role, PublicOrganization? organization = null, string permission = "Beredskapsansatt")
        {
            var account = await context.Accounts.SingleOrDefaultAsync(account => account.email == email);
            if (account != null)
            {
                return account;
            }

            account = new Account
            {
                email = email,
                contact_name = name,
                phone = "12345678",
                account_type = role,
                email_confirmed = true
            };
            account.password_hash = new PasswordHasher<Account>().HashPassword(account, "Gruppe2-Test-2026!");
            context.Accounts.Add(account);
            await context.SaveChangesAsync();
            if (role == "Private")
            {
                context.ProviderAccounts.Add(new ProviderAccount { account_id = account.account_id, contact_name = name, phone = account.phone, address = string.Empty });
            }

            if (role == "Public")
            {
                context.PublicActorAccounts.Add(new PublicActorAccount { account_id = account.account_id, public_organization_id = organization!.public_organization_id, permission = permission });
            }

            await context.SaveChangesAsync();
            return account;
        }

        if (!await context.Accounts.AnyAsync(account => account.account_type == "Platform"))
        {
            await User("eier@example.test", "Systemansvarlig", "Platform");
        }
        await User("admin@sor.example.test", "Organisasjonsadministrator Sør", "Public", south, "Organisasjonsadministrator");
        var actor = await User("beredskap@sor.example.test", "Beredskapsansatt Sør", "Public", south);
        await User("leser@sor.example.test", "Lesetilgang Sør", "Public", south, "Lesetilgang");
        await User("admin@nord.example.test", "Organisasjonsadministrator Nord", "Public", north, "Organisasjonsadministrator");
        var provider = await User("privat@example.test", "Privat leverandør", "Private");
        await User("bedrift@example.test", "Bedriftsleverandør", "Private");
        if (!await context.Resources.AnyAsync(resource => resource.account_id == provider.account_id))
        {
            var resource = new Resource
            {
                account_id = provider.account_id,
                resource_name = "Eksempel – aggregat",
                resource_type_id = 7,
                description = "Lokale eksempeldata til utprøving.",
                contact_details = "Kontakt via testkonto",
                verification_status = "Ikke verifisert",
                Location = new Location
                {
                    address_or_area = "Kristiansand",
                    latitude = 58.1467m,
                    longitude = 7.9956m
                }
            };
            context.Resources.Add(resource);
            await context.SaveChangesAsync();
            context.AvailabilityPeriods.Add(new Availability { resource_id = resource.resource_id, available_from = DateTime.Today.AddDays(1), available_to = DateTime.Today.AddMonths(2) });
            await context.SaveChangesAsync();
        }

        if (!await context.ResourceNeeds.AnyAsync(need => need.public_actor_account_id == actor.account_id))
        {
            context.ResourceNeeds.Add(new ResourceNeed { public_actor_account_id = actor.account_id, title = "Eksempel – strøm til samlingspunkt", resource_type_id = 7, description = "Lokale eksempeldata til utprøving.", required_start = DateTime.Today.AddDays(2), required_end = DateTime.Today.AddDays(3), Location = new Location { address_or_area = "Kristiansand", latitude = 58.15m, longitude = 8.01m } });
            await context.SaveChangesAsync();
        }
    }
}
