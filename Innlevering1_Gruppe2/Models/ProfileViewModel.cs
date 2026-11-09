using System.ComponentModel.DataAnnotations;
using Reachout.Models.Entities;

namespace Reachout.Models;
public class ProfileViewModel
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, Phone, MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Address { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Organization { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? OrganizationNumber { get; set; } = string.Empty;
    public string ProviderType { get; set; } = "Privatperson";
    public string? Email { get; set; } = string.Empty;
}
