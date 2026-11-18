using System.ComponentModel.DataAnnotations;
using Reachout.Models;

namespace Reachout.Tests;

/* Testklasse for å validere ViewModels.
 Tester ulike scenarier for å sikre at validering fungerer som forventet. */

public class ViewModelValidationTests
{
    /* Test for å sikre at ResourceEditViewModel aksepterer gyldige koordinater innenfor grensene,
     og at ingen valideringsfeil oppstår.*/
    [Theory]
    [InlineData(-90, -180)]
    [InlineData(0, 0)]
    [InlineData(90, 180)]
    public void ResourceEditViewModel_AcceptsCoordinateBoundaries(
        double latitude,
        double longitude)
    {
        var model = ValidResourceEditModel();
        model.Latitude = latitude;
        model.Longitude = longitude;

        Assert.Empty(Validate(model));
    }

    /* Test for å sikre at ResourceEditViewModel avviser koordinater utenfor grensene,
     og at valideringsfeil oppstår.*/
    [Theory]
    [InlineData(-90.01, 0)]
    [InlineData(90.01, 0)]
    [InlineData(0, -180.01)]
    [InlineData(0, 180.01)]
    public void ResourceEditViewModel_RejectsCoordinatesOutsideBounds(
        double latitude,
        double longitude)
    {
        var model = ValidResourceEditModel();
        model.Latitude = latitude;
        model.Longitude = longitude;

        Assert.NotEmpty(Validate(model));
    }

    /* Test for å sikre at ResourceEditViewModel krever navn, ressurskategori og koordinater,
     og at valideringsfeil oppstår når disse feltene mangler.*/
    [Fact]
    public void ResourceEditViewModel_RequiresNameTypeAndCoordinates()
    {
        var errors = Validate(new ResourceEditViewModel());

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(ResourceEditViewModel.Name)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(ResourceEditViewModel.ResourceTypeId)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(ResourceEditViewModel.Latitude)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(ResourceEditViewModel.Longitude)));
    }

    /* Test for å sikre at ResourceEditViewModel avviser navn og
     beskrivelse som overskrider grensen for antall tegn */
    [Fact]
    public void ResourceEditViewModel_RejectsNameAndDescriptionOverMaximumLength()
    {
        var model = ValidResourceEditModel();
        model.Name = new string('n', 201);
        model.Description = new string('d', 4001);

        var errors = Validate(model);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(ResourceEditViewModel.Name)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(ResourceEditViewModel.Description)));
    }

    /* Test for å sikre at ResourceEditViewModel krever gyldige ressursfelt,
     og at valideringsfeil oppstår når feltene er ugyldige.*/
    [Fact]
    public void ResourceEditViewModel_RequiresValidResourceFields()
    {
        var errors = Validate(new ResourceEditViewModel
        {
            Name = "Traktor",
            ResourceTypeId = 1,
            Latitude = 91,
            Longitude = 10
        });

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(ResourceEditViewModel.Latitude)));
    }

    /* Test for å sikre at AccountRegistrationViewModel aksepterer gyldige detaljer,
     og at ingen valideringsfeil oppstår.*/
    [Fact]
    public void AccountRegistrationViewModel_AcceptsMatchingValidDetails()
    {
        var model = new AccountRegistrationViewModel
        {
            ContactName = "Ola Nordmann",
            Phone = "+47 123 45 678",
            Email = "ola@example.com",
            Password = "hemmelig123",
            ConfirmPassword = "hemmelig123"
        };

        Assert.Empty(Validate(model));
    }

    /* Test for å sikre at AccountRegistrationViewModel avviser manglende eller ugyldige detaljer,
     og at valideringsfeil oppstår.*/
    [Theory]
    [InlineData("", "12345678", "ola@example.com", "hemmelig123", "hemmelig123")]
    [InlineData("Ola", "12345678", "ikke-en-epost", "hemmelig123", "hemmelig123")]
    [InlineData("Ola", "12345678", "ola@example.com", "kort", "kort")]
    [InlineData("Ola", "12345678", "ola@example.com", "hemmelig123", "noe-annet")]
    public void AccountRegistrationViewModel_RejectsMissingOrInvalidDetails(
        string contactName,
        string phone,
        string email,
        string password,
        string confirmPassword)
    {
        var model = new AccountRegistrationViewModel
        {
            ContactName = contactName,
            Phone = phone,
            Email = email,
            Password = password,
            ConfirmPassword = confirmPassword
        };

        Assert.NotEmpty(Validate(model));
    }

    /* Test for å sikre at LoginViewModel krever gyldig e-post og passord,
     og at valideringsfeil oppstår når disse feltene mangler eller er ugyldige.*/
    [Fact]
    public void LoginViewModel_RequiresValidEmailAndPassword()
    {
        Assert.NotEmpty(Validate(new LoginViewModel
        {
            Email = "invalid",
            Password = string.Empty
        }));
    }

    private static ResourceEditViewModel ValidResourceEditModel() => new()
    {
        Name = "Traktor",
        Description = "En fungerende traktor",
        ResourceTypeId = 1,
        Latitude = 59.9,
        Longitude = 10.7
    };

    /* Validerer et ViewModel-objekt og returnerer en liste over valideringsfeil, hvis noen.*/
    private static IReadOnlyList<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            results,
            validateAllProperties: true);
        return results;
    }
}
