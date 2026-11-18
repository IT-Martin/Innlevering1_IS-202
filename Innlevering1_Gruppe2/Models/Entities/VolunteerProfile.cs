using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities;
public class VolunteerProfile
{
    public int volunteer_profile_id { get; set; }
    public int account_id { get; set; }
    public ProviderAccount ProviderAccount { get; set; } = null!;

    [Required, MaxLength(300)]
    public string service_area { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string volunteer_role { get; set; } = "Frivillig";

    [MaxLength(2000)]
    public string additional_information { get; set; } = string.Empty;
    public bool archived { get; set; }
    public ICollection<VolunteerProfileSkill> VolunteerProfileSkills { get; set; } = new List<VolunteerProfileSkill>();
}
