using System.ComponentModel.DataAnnotations;
using Reachout.Models.Entities;

namespace Reachout.Models;
public class OrganizationFormViewModel
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Type { get; set; } = "Kommune";

    [Required, RegularExpression(@"\d{9}")]
    public string Number { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Domain { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Contact { get; set; } = string.Empty;

    [Phone, MaxLength(50)]
    public string? Phone { get; set; } = string.Empty;
}
