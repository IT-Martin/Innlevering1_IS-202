using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities;
public class AccountToken
{
    public int account_token_id { get; set; }

    [MaxLength(100)]
    public string security_stamp { get; set; } = string.Empty;
    public int account_id { get; set; }
    public Account Account { get; set; } = null!;

    [MaxLength(100)]
    public string token_hash { get; set; } = string.Empty;

    [MaxLength(30)]
    public string purpose { get; set; } = string.Empty;
    public DateTime expires_at { get; set; }

    [ConcurrencyCheck]
    public DateTime? used_at { get; set; }
}
