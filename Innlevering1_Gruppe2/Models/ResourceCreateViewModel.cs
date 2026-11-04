using System.ComponentModel.DataAnnotations;

namespace Innlevering1_Gruppe2.Models;

public class ResourceCreateViewModel
{
    [Required(ErrorMessage = "Skriv inn navnet på ressursen.")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Velg type ressurs.")]
    public int? ResourceTypeId { get; set; }

    [Required(ErrorMessage = "Velg en posisjon på kartet.")]
    [Range(-90, 90, ErrorMessage = "Latitude må være mellom -90 og 90.")]
    public double? Latitude { get; set; }

    [Required(ErrorMessage = "Velg en posisjon på kartet.")]
    [Range(-180, 180, ErrorMessage = "Longitude må være mellom -180 og 180.")]
    public double? Longitude { get; set; }
}
