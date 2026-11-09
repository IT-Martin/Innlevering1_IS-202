using Reachout.Controllers;
using Reachout.Models;
using Reachout.Models.Entities;
using Reachout.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Reachout.Tests;

public class LoginControllerTests
/*
 Tester for å sikre at LoginController fungerer som forventet, inkludert
 håndtering av innlogging med ukjent e-post eller feil passord,
 samt riktig feilmelding i begge tilfeller.
 */
{
    [Fact]
    public async Task Login_Post_RejectsUnknownEmail()
    /* Test for å sikre at innlogging med en ukjent e-postadresse gir riktig feilmelding. */
    {
        await using var context = TestDatabase.CreateContext();
        var controller = new LoginController(context, CreateAccountService(context));

        var result = await controller.Index(new LoginViewModel
        {
            Email = "missing@example.com",
            Password = "password123"
        });

        Assert.IsType<ViewResult>(result);
        Assert.Contains(controller.ModelState[string.Empty]!.Errors,
            error => error.ErrorMessage == "Feil e-post eller passord.");
    }

    [Fact]
    public async Task Login_Post_RejectsIncorrectPassword()
    /* Test for å sikre at innlogging med en korrekt e-postadresse, men feil passord,
       gir riktig feilmelding.*/
    {
        await using var context = TestDatabase.CreateContext();
        var account = await context.Accounts.FindAsync(1);
        Assert.NotNull(account);
        account.password_hash = new PasswordHasher<Account>()
            .HashPassword(account, "correct-password");
        await context.SaveChangesAsync();

        var controller = new LoginController(context, CreateAccountService(context));

        var result = await controller.Index(new LoginViewModel
        {
            Email = " OWNER@example.com ",
            Password = "wrong-password"
        });

        Assert.IsType<ViewResult>(result);
        Assert.Contains(controller.ModelState[string.Empty]!.Errors,
            error => error.ErrorMessage == "Feil e-post eller passord.");
    }

    private static AccountService CreateAccountService(Reachout.DataAccess.ApplicationDbContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["LocalPreview"] = "true" })
            .Build();
        return new AccountService(context, configuration);
    }
}
