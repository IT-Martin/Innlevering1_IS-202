using System.ComponentModel.DataAnnotations;

namespace Reachout.Models;
public class ResourceEditViewModel
{
    public string AvailabilityStatus { get; set; } = "Etter avtale";

    [MaxLength(500)]
    public string? Address { get; set; } = string.Empty;
    public DateTime Start { get; set; } = DateTime.Today.AddDays(1);
    public DateTime End { get; set; } = DateTime.Today.AddMonths(1);
    public IFormFile? Photo { get; set; }
    public bool RemovePhoto { get; set; }
    public bool HasPhoto { get; set; }
    public int Id { get; set; }

    [Required(ErrorMessage = "Skriv inn navnet på ressursen.")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Velg type ressurs.")]
    public int? ResourceTypeId { get; set; }

    [Required(ErrorMessage = "Velg en posisjon på kartet.")]
    [Range(-90d, 90d, ErrorMessage = "Breddegrad må være mellom -90 og 90.")]
    public double? Latitude { get; set; }

    [Required(ErrorMessage = "Velg en posisjon på kartet.")]
    [Range(-180d, 180d, ErrorMessage = "Lengdegrad må være mellom -180 og 180.")]
    public double? Longitude { get; set; }
}
