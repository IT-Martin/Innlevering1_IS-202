using System.ComponentModel.DataAnnotations;

namespace Innlevering1_Gruppe2.Models.Entities
{
    public class ProviderAccount
    {
        public int account_id { get; set; } // PK og eventuelt FK

        [Required]
        [MaxLength(100)]
        public string provider_type { get; set; }

        [Required]
        [MaxLength(200)]
        public string contact_name { get; set; }

        [Required]
        [MaxLength(50)]
        public string? phone { get; set; }

        [Required]
        [MaxLength(100)]
        public string? address { get; set; }

        
        [MaxLength(200)]
        public string organization_name { get; set; } = string.Empty;

        
        [MaxLength(100)]
        public string organization_number { get; set; } = string.Empty;

    }
}