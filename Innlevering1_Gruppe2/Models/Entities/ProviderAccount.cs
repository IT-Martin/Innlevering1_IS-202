using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities
{
    public class ProviderAccount
    {
        public int account_id { get; set; }
        public Account Account { get; set; } = null!;
        public ICollection<Resource> Resources { get; set; } = new List<Resource>();

        [Required]
        [MaxLength(100)]
        public string provider_type { get; set; } = "Privatperson";

        [Required]
        [MaxLength(200)]
        public string contact_name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string? phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string? address { get; set; } = string.Empty;

        [MaxLength(200)]
        public string organization_name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string organization_number { get; set; } = string.Empty;
    }
}
