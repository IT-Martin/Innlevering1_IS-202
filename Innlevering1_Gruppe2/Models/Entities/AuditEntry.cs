using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities;
public class AuditEntry
{
    public int audit_entry_id { get; set; }
    public int? account_id { get; set; }

    [MaxLength(80)]
    public string action { get; set; } = string.Empty;

    [MaxLength(100)]
    public string target { get; set; } = string.Empty;
    public DateTime occurred_at { get; set; } = DateTime.UtcNow;
}
