using System.ComponentModel.DataAnnotations;

namespace Innlevering1_Gruppe2.Models;

public class ResourceEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Skriv inn navnet på ressursen.")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Velg type ressurs.")]
    public int? ResourceTypeId { get; set; }

    [Required(ErrorMessage = "Velg en posisjon på kartet.")]
    [Range(-90.0, 90.0, ErrorMessage = "Latitude må være mellom -90 og 90.")]
    public double? Latitude { get; set; }

    [Required(ErrorMessage = "Velg en posisjon på kartet.")]
    [Range(-180.0, 180.0, ErrorMessage = "Longitude må være mellom -180 og 180.")]
    public double? Longitude { get; set; }
}
