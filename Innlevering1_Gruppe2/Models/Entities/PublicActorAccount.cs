using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities;
public class PublicActorAccount
{
    public int account_id { get; set; }
    public Account Account { get; set; } = null!;
    public int public_organization_id { get; set; }
    public PublicOrganization PublicOrganization { get; set; } = null!;

    [MaxLength(30)]
    public string permission { get; set; } = "Beredskapsansatt";
}
