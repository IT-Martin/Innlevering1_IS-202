using System.ComponentModel.DataAnnotations;
using Reachout.Models.Entities;

namespace Reachout.Models;
public class OrganizationAdminViewModel
{
    public bool Platform { get; set; }
    public List<PublicOrganization> Organizations { get; set; } = new();
    public List<PublicActorAccount> Members { get; set; } = new();
    public List<OrganizationInvitation> Invitations { get; set; } = new();
    public List<AuditEntry> Audit { get; set; } = new();
}
