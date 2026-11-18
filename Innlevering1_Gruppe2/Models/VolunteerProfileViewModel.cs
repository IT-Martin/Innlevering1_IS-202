using System.ComponentModel.DataAnnotations;
using Reachout.Models.Entities;

namespace Reachout.Models;
public class VolunteerProfileViewModel
{
    public int Id { get; set; }
    public int? NeedId { get; set; }

    [Required, MaxLength(300)]
    public string Area { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Role { get; set; } = "Frivillig";

    [MaxLength(2000)]
    public string? Information { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Skills { get; set; } = string.Empty;
    public bool Archived { get; set; }
}
