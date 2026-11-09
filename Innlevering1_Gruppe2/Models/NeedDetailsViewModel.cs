using System.ComponentModel.DataAnnotations;
using Reachout.Models.Entities;

namespace Reachout.Models;
public class NeedDetailsViewModel
{
    public ResourceNeed Need { get; set; } = null!;
    public List<Reservation> Reservations { get; set; } = new();
    public List<VolunteerInterest> Interests { get; set; } = new();
    public List<Resource> Resources { get; set; } = new();
    public VolunteerProfile? MyVolunteerProfile { get; set; }
    public VolunteerInterest? MyActiveInterest { get; set; }
    public bool CanEdit { get; set; }
}
