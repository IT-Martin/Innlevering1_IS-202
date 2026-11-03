using System.ComponentModel.DataAnnotations;

namespace Innlevering1_Gruppe2.Models;

public class AccountRegistrationViewModel
{
    [Required(ErrorMessage = "Navn er påkrevd.")]
    [MaxLength(200, ErrorMessage = "Navnet kan ikke være lengre enn 200 tegn.")]
    public string ContactName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefonnummer er påkrevd.")]
    [Phone(ErrorMessage = "Skriv inn et gyldig telefonnummer.")]
    [MaxLength(50, ErrorMessage = "Telefonnummeret kan ikke være lengre enn 50 tegn.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-post er påkrevd.")]
    [EmailAddress(ErrorMessage = "Skriv inn en gyldig e-postadresse.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Passord er påkrevd.")]
    [MinLength(8, ErrorMessage = "Passordet må være minst 8 tegn.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bekreft passordet.")]
    [Compare(nameof(Password), ErrorMessage = "Passordene er ikke like.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}