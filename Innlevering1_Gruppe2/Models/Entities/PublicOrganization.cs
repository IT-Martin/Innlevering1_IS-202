using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities;
public class PublicOrganization
{
    public int public_organization_id { get; set; }

    [Required, MaxLength(200)]
    public string organization_name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string organization_type { get; set; } = "Kommune";

    [Required, MaxLength(20)]
    public string organization_number { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string email_domain { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string contact_email { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string contact_name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string phone { get; set; } = string.Empty;

    [MaxLength(30)]
    public string approval_status { get; set; } = "Venter";

    [MaxLength(1000)]
    public string verification { get; set; } = string.Empty;
    public DateTime created_at { get; set; } = DateTime.UtcNow;
}
