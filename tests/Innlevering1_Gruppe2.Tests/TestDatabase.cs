using Reachout.DataAccess;
using Reachout.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Reachout.Tests;

/* Testklasse for å opprette testdatabaser.
 Disse testene sikrer at vi har en isolert testmiljø med kjente data.*/
internal static class TestDatabase
{
    // Lager en enkeltstående testdatabase for hver test, slik at de ikke påvirker hverandre.
    public static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"tests-{Guid.NewGuid()}")
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        context.Accounts.AddRange(
            new Account
            {
                account_id = 1,
                account_type = "Private",
                email = "owner@example.com",
                contact_name = "Resource Owner",
                phone = "12345678",
                password_hash = "hashed-password"
            },
            new Account
            {
                account_id = 2,
                account_type = "Private",
                email = "other@example.com",
                contact_name = "Other Owner",
                phone = "87654321",
                password_hash = "hashed-password"
            });
        context.SaveChanges();

        return context;
    }

    public static Resource CreateResource(
        ApplicationDbContext context,
        int accountId = 1,
        string name = "Testressurs")
    {
        var resource = new Resource
        {
            account_id = accountId,
            resource_name = name,
            description = "Beskrivelse",
            contact_details = "Kontakt",
            verification_status = "Pending",
            resource_type_id = 1,
            ResourceType = context.ResourceTypes.Single(type => type.resource_type_id == 1),
            Location = new Location
            {
                address_or_area = "Teststed",
                latitude = 59.9m,
                longitude = 10.7m
            }
        };

        context.Resources.Add(resource);
        context.SaveChanges();
        return resource;
    }
}
