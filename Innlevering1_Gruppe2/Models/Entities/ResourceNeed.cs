using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities;
public class ResourceNeed
{
    public int resource_need_id { get; set; }
    public int public_actor_account_id { get; set; }
    public PublicActorAccount PublicActorAccount { get; set; } = null!;
    public int resource_type_id { get; set; }
    public ResourceType ResourceType { get; set; } = null!;
    public int location_id { get; set; }
    public Location Location { get; set; } = null!;

    [Required, MaxLength(200)]
    public string title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string description { get; set; } = string.Empty;

    [MaxLength(30)]
    public string priority { get; set; } = "Normal";

    [MaxLength(30)]
    public string status { get; set; } = "Ny";
    public DateTime required_start { get; set; }
    public DateTime required_end { get; set; }
    public DateTime? deadline { get; set; }
    public bool archived { get; set; }
}
