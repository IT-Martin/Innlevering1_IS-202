using System.ComponentModel.DataAnnotations;
using Reachout.Models.Entities;

namespace Reachout.Models;
public class NeedFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Skriv en kort tittel på behovet.")]
    [MaxLength(200, ErrorMessage = "Tittelen kan ha høyst 200 tegn.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000, ErrorMessage = "Beskrivelsen kan ha høyst 4000 tegn.")]
    public string? Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Velg en kategori.")]
    public int? ResourceTypeId { get; set; }

    [Required(ErrorMessage = "Skriv inn en adresse eller et område.")]
    [MaxLength(500, ErrorMessage = "Adressen eller området kan ha høyst 500 tegn.")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Velg en plassering på kartet.")]
    [Range(-90d, 90d, ErrorMessage = "Velg en gyldig breddegrad på kartet.")]
    public double? Latitude { get; set; }

    [Required(ErrorMessage = "Velg en plassering på kartet.")]
    [Range(-180d, 180d, ErrorMessage = "Velg en gyldig lengdegrad på kartet.")]
    public double? Longitude { get; set; }
    public string Priority { get; set; } = "Normal";
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public DateTime? Deadline { get; set; }
    public List<ResourceType> Types { get; set; } = new();
}
