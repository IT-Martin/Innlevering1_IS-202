using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities;
public class OrganizationInvitation
{
    public int invitation_id { get; set; }
    public int public_organization_id { get; set; }
    public PublicOrganization PublicOrganization { get; set; } = null!;

    [Required, MaxLength(255)]
    public string email { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string token_hash { get; set; } = string.Empty;

    [MaxLength(30)]
    public string permission { get; set; } = "Beredskapsansatt";
    public DateTime expires_at { get; set; } = DateTime.UtcNow.AddDays(2);
    public DateTime? used_at { get; set; }
    public bool revoked { get; set; }
}
