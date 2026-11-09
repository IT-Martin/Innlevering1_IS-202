using System.ComponentModel.DataAnnotations;
using Reachout.Models.Entities;

namespace Reachout.Models;
public class ReservationFormViewModel
{
    public int ResourceId { get; set; }
    public int NeedId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    [MaxLength(2000)]
    public string? Message { get; set; } = string.Empty;
}
