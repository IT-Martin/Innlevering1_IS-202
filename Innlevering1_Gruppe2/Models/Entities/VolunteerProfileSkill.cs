using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities;
public class VolunteerProfileSkill
{
    public int volunteer_profile_id { get; set; }
    public VolunteerProfile VolunteerProfile { get; set; } = null!;
    public int skill_id { get; set; }
    public Skill Skill { get; set; } = null!;
}
