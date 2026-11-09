using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities;
public class VolunteerInterest
{
    public int interest_id { get; set; }
    public int volunteer_profile_id { get; set; }
    public VolunteerProfile VolunteerProfile { get; set; } = null!;
    public int resource_need_id { get; set; }
    public ResourceNeed ResourceNeed { get; set; } = null!;

    [MaxLength(2000)]
    public string message { get; set; } = string.Empty;

    [MaxLength(40)]
    public string interest_status { get; set; } = "Venter på svar";
    public DateTime submitted_at { get; set; } = DateTime.UtcNow;
}
