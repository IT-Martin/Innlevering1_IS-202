using System.ComponentModel.DataAnnotations;

namespace Reachout.Models
{
    public class LoginViewModel
    {
        public string Role { get; set; } = "Private";

        [Required(ErrorMessage = "E-post er påkrevd.")]
        [EmailAddress(ErrorMessage = "Skriv inn en gyldig e-postadresse.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Passord er påkrevd.")]
        [MaxLength(128, ErrorMessage = "Passordet kan ha høyst 128 tegn.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
