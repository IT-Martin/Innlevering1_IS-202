using System.ComponentModel.DataAnnotations;

namespace Reachout.Models;
public class OrganizationFormViewModel : IValidatableObject
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "Oppgi organisasjonsnummeret.")]
    public string Number { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Contact { get; set; } = string.Empty;

    [Phone, MaxLength(50)]
    public string? Phone { get; set; }

    public string NormalizedNumber => string.Concat(Number.Where(character => !char.IsWhiteSpace(character)));

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Type is not ("Kommune" or "Fylkeskommune" or "Annen offentlig virksomhet"))
        {
            yield return new ValidationResult("Velg en offentlig organisasjonstype.", new[] { nameof(Type) });
        }

        var number = NormalizedNumber;
        if (number.Length != 9 || number.Any(character => character is < '0' or > '9'))
        {
            yield return new ValidationResult("Organisasjonsnummeret må ha ni sifre.", new[] { nameof(Number) });
            yield break;
        }

        int[] weights = { 3, 2, 7, 6, 5, 4, 3, 2 };
        var sum = 0;
        for (var index = 0; index < weights.Length; index++)
        {
            sum += (number[index] - '0') * weights[index];
        }

        var checkDigit = (11 - sum % 11) % 11;
        if (number == "000000000" || checkDigit != number[8] - '0')
        {
            yield return new ValidationResult("Kontroller organisasjonsnummeret. Det har ugyldig kontrollsiffer.", new[] { nameof(Number) });
        }
    }
}
