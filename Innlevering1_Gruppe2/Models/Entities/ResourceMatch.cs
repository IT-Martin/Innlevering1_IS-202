using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities;
public class ResourceMatch
{
    public int resource_match_id { get; set; }
    public int resource_id { get; set; }
    public Resource Resource { get; set; } = null!;
    public int resource_need_id { get; set; }
    public ResourceNeed ResourceNeed { get; set; } = null!;
    public int public_actor_account_id { get; set; }
    public PublicActorAccount PublicActorAccount { get; set; } = null!;

    [MaxLength(40)]
    public string match_status { get; set; } = "Venter på svar";
    public DateTime assessed_at { get; set; } = DateTime.UtcNow;
}
