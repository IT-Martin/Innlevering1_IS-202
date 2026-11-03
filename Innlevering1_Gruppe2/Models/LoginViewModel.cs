using System.ComponentModel.DataAnnotations;

namespace Innlevering1_Gruppe2.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-post er påkrevd.")]
        [EmailAddress(ErrorMessage = "Skriv inn en gyldig e-postadresse.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Passord er påkrevd.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}