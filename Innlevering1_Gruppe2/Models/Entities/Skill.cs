using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities;
public class Skill
{
    public int skill_id { get; set; }

    [Required, MaxLength(200)]
    public string name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string description { get; set; } = string.Empty;
}
