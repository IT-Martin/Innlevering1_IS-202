using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities;
public class Reservation
{
    public int reservation_id { get; set; }
    public int resource_match_id { get; set; }
    public ResourceMatch ResourceMatch { get; set; } = null!;
    public int public_actor_account_id { get; set; }

    [MaxLength(40)]
    public string reservation_status { get; set; } = "Venter på svar";
    public DateTime requested_start { get; set; }
    public DateTime requested_end { get; set; }

    [MaxLength(2000)]
    public string message { get; set; } = string.Empty;
}
