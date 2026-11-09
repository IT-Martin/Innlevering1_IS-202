using System.ComponentModel.DataAnnotations;
using Reachout.Models.Entities;

namespace Reachout.Models;
public class AvailabilityViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Volunteer { get; set; }
    public bool Archived { get; set; }
    public List<Availability> Periods { get; set; } = new();
}
