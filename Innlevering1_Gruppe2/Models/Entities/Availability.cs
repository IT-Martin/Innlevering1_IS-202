using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities;
public class Availability
{
    public int availability_id { get; set; }
    public int? resource_id { get; set; }
    public Resource? Resource { get; set; }
    public int? volunteer_profile_id { get; set; }
    public VolunteerProfile? VolunteerProfile { get; set; }
    public DateTime available_from { get; set; }
    public DateTime available_to { get; set; }
}
