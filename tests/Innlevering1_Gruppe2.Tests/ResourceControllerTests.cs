using System.Security.Claims;
using Innlevering1_Gruppe2.Controllers;
using Innlevering1_Gruppe2.DataAccess.Repositories;
using Innlevering1_Gruppe2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Innlevering1_Gruppe2.Tests;


/* Tester for å sikre at ResourceController fungerer som forventet,
inkludert registrering, redigering og visning av ressurser, samt håndtering
av tilgangskontroll basert på kontoeierskap. */

public class ResourceControllerTests
{
    /* Test for å sikre at registrering av en ny ressurs med gyldige data fungerer som forventet,
     og at ressursen lagres med trimmet navn og beskrivelse.*/
    [Fact]
    public async Task Register_Post_CreatesTrimmedResourceAndRedirectsToResult()
    
    {
        await using var context = TestDatabase.CreateContext();
        var controller = CreateController(context, 1);
        var model = new ResourceCreateViewModel
        {
            Name = "  Traktor  ",
            Description = "  Kan brukes  ",
            ResourceTypeId = 1,
            Latitude = 59.9123456,
            Longitude = 10.7123456
        };

        var result = await controller.Register(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ResourceController.Result), redirect.ActionName);
        var savedResource = Assert.Single(context.Resources);
        Assert.Equal("Traktor", savedResource.resource_name);
        Assert.Equal("Kan brukes", savedResource.description);
        Assert.Equal("Pending", savedResource.verification_status);
        Assert.Equal("Navn: Resource Owner, Telefon: 12345678", savedResource.contact_details);
        Assert.Equal("Koordinater: 59.912346, 10.712346", savedResource.Location.address_or_area);
        Assert.Equal(savedResource.resource_id, redirect.RouteValues?["id"]);
        // Verifiser at ressursen er knyttet til riktig konto
    }

    /* Test for å sikre at registrering av en ny ressurs med ugyldige data gir riktig feilmelding,
     og at ressursen ikke lagres i databasen.*/
    [Fact]
    public async Task Register_Post_InvalidModelRedisplaysFormWithoutSaving()
    {
        await using var context = TestDatabase.CreateContext();
        var controller = CreateController(context, 1);
        var model = new ResourceCreateViewModel();
        controller.ModelState.AddModelError(nameof(model.Name), "Skriv inn navnet på ressursen.");

        var result = await controller.Register(model);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(model, view.Model);
        Assert.Empty(context.Resources);
        // Verifiser at ressursen ikke er lagret i databasen
    }
    
    /* Test for å sikre at registrering av en ny ressurs med en ukjent ressurs-type gir riktig feilmelding,
     og at ressursen ikke lagres i databasen.*/
    [Fact]
    public async Task Register_Post_RejectsUnknownResourceType()
    {
        await using var context = TestDatabase.CreateContext();
        var controller = CreateController(context, 1);
        var model = ValidCreateModel();
        model.ResourceTypeId = 999;

        var result = await controller.Register(model);

        Assert.IsType<ViewResult>(result);
        Assert.True(controller.ModelState.ContainsKey(nameof(model.ResourceTypeId)));
        Assert.Empty(context.Resources);
        // Verifiser at ressursen ikke er lagret i databasen
    }

    /* Test for å sikre at registrering av en ny ressurs uten en gyldig konto krever autentisering,
     og at ressursen ikke lagres i databasen.*/
    [Fact]
    public async Task Register_Post_ChallengesRequestWithoutAccountClaim()
    {
        await using var context = TestDatabase.CreateContext();
        var controller = CreateController(context, null);

        var result = await controller.Register(ValidCreateModel());

        Assert.IsType<ChallengeResult>(result);
        Assert.Empty(context.Resources);
        // Verifiser at ressursen ikke er lagret i databasen
    }

    /* Test for å sikre at kartvisning returnerer ressurser som tilhører den innloggede kontoen,
     med forventet ikon og koordinater.*/
    [Fact]
    public async Task Map_ReturnsCurrentAccountsResourcesWithExpectedIconAndCoordinates()
    {
        await using var context = TestDatabase.CreateContext();
        var resource = TestDatabase.CreateResource(context, name: "Traktor");
        TestDatabase.CreateResource(context, accountId: 2, name: "Annen konto");
        var controller = CreateController(context, 1);

        var result = await controller.Map();

        var view = Assert.IsType<ViewResult>(result);
        var item = Assert.Single(Assert.IsAssignableFrom<IEnumerable<RessursViewModel>>(view.Model));
        Assert.Equal(resource.resource_id, item.Id);
        Assert.Equal("Traktor", item.RessursNavn);
        Assert.Equal("fa-solid fa-tractor", item.TypeIcon);
        Assert.Equal(59.9, item.Latitude);
        Assert.Equal(10.7, item.Longitude);
        // Verifiser at kartvisningen returnerer ressurser som tilhører den innloggede kontoen
    }

    /* Test for å sikre at redigering av en ressurs som tilhører en annen konto returnerer NotFound,
     og at ressursen ikke blir endret i databasen.*/
    [Fact]
    public async Task Edit_Get_ReturnsNotFoundForAnotherAccountsResource()
    {
        await using var context = TestDatabase.CreateContext();
        var resource = TestDatabase.CreateResource(context, accountId: 2);
        var controller = CreateController(context, 1);

        var result = await controller.Edit(resource.resource_id);

        Assert.IsType<NotFoundResult>(result);
        // Verifiser at ressursen ikke blir endret i databasen
    }

    /* Test for å sikre at redigering av en ressurs som tilhører den innloggede kontoen oppdaterer ressursen med trimmet navn og beskrivelse,
     og at brukeren blir omdirigert til MyResources-siden.*/
    [Fact]
    public async Task Edit_Post_UpdatesOwnedResourceAndRedirectsToMyResources()
    {
        await using var context = TestDatabase.CreateContext();
        var resource = TestDatabase.CreateResource(context);
        var controller = CreateController(context, 1);
        var model = new ResourceEditViewModel
        {
            Id = resource.resource_id,
            Name = "  Nytt navn ",
            Description = "  Ny beskrivelse ",
            ResourceTypeId = 1,
            Latitude = 60.123456,
            Longitude = 11.654321
        };

        var result = await controller.Edit(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ResourceController.MyResources), redirect.ActionName);
        Assert.Equal("Nytt navn", resource.resource_name);
        Assert.Equal("Ny beskrivelse", resource.description);
        Assert.Equal(60.123456m, resource.Location.latitude);
        Assert.Equal(11.654321m, resource.Location.longitude);
        Assert.Equal("Koordinater: 60.123456, 11.654321", resource.Location.address_or_area);
        // Verifiser at ressursen er oppdatert med trimmet navn og beskrivelse
    }

    /* Test for å sikre at redigering av en ressurs som tilhører en annen konto returnerer NotFound,
     og at ressursen ikke blir endret i databasen.*/
    [Fact]
    public async Task Edit_Post_DoesNotUpdateAnotherAccountsResource()
    {
        await using var context = TestDatabase.CreateContext();
        var resource = TestDatabase.CreateResource(context, accountId: 2);
        var controller = CreateController(context, 1);

        var result = await controller.Edit(new ResourceEditViewModel
        {
            Id = resource.resource_id,
            Name = "Forsøk på endring",
            Description = "Ny tekst",
            ResourceTypeId = 1,
            Latitude = 60,
            Longitude = 11
        });

        Assert.IsType<NotFoundResult>(result);
        Assert.Equal("Testressurs", resource.resource_name);
        // Verifiser at ressursen ikke blir endret i databasen
    }

    /* Test for å sikre at visning av en ressurs som tilhører en annen konto omdirigerer brukeren til kartvisningen,
     og at ressursen ikke blir vist.*/
    [Fact]
    public async Task Result_Get_RedirectsToMapWhenResourceDoesNotBelongToAccount()
    {
        await using var context = TestDatabase.CreateContext();
        var resource = TestDatabase.CreateResource(context, accountId: 2);
        var controller = CreateController(context, 1);

        var result = await controller.Result(resource.resource_id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ResourceController.Map), redirect.ActionName);
        // Verifiser at ressursen ikke blir vist
    }

    /* Statisk hjelpefunksjon for å opprette en ResourceController-instans for testing */
    private static ResourceController CreateController(
        Innlevering1_Gruppe2.DataAccess.ApplicationDbContext context,
        int? accountId)
    {
        var controller = new ResourceController(new ResourceRepository(context), context);
        var claims = accountId is null
            ? Array.Empty<Claim>()
            : new[] { new Claim(ClaimTypes.NameIdentifier, accountId.Value.ToString()) };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"))
            }
        };
        return controller;
    }

    /* Statisk hjelpefunksjon for å opprette en gyldig ResourceCreateViewModel for testing */
    private static ResourceCreateViewModel ValidCreateModel() => new()
    {
        Name = "Traktor",
        Description = "Beskrivelse",
        ResourceTypeId = 1,
        Latitude = 59.9,
        Longitude = 10.7
    };
}
