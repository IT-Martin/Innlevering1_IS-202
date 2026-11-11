using System.ComponentModel.DataAnnotations;
using Reachout.Models;

namespace Reachout.Tests;

public class OrganizationFormViewModelTests
{
    [Theory]
    [InlineData("123 456 785", true)]
    [InlineData("123456785", true)]
    [InlineData("123456784", false)]
    [InlineData("12345678", false)]
    [InlineData("000000000", false)]
    public void ValidatesOrganizationNumber(string number, bool expected)
    {
        var model = new OrganizationFormViewModel
        {
            Name = "Eksempelkommune",
            Type = "Kommune",
            Number = number,
            Email = "kontakt@example.test",
            Contact = "Kontaktperson"
        };

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), null, true);

        Assert.Equal(expected, valid);
    }
}
