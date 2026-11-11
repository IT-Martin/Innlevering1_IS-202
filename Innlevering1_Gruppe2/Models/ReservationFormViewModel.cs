using System.ComponentModel.DataAnnotations;

namespace Reachout.Models;
public class ReservationFormViewModel
{
    public int ResourceId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Velg et åpent behov.")]
    public int NeedId { get; set; }
    [Required(ErrorMessage = "Velg starttid.")]
    public DateTime? Start { get; set; }
    [Required(ErrorMessage = "Velg sluttid.")]
    public DateTime? End { get; set; }

    [MaxLength(2000)]
    public string? Message { get; set; } = string.Empty;
}
