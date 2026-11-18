using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities
{
    public class Resource
    {
        public int resource_id { get; set; } // PK
        public int account_id { get; set; } // FK til eier
        public int resource_type_id { get; set; } // FK
        public int location_id { get; set; } // FK

        [Required]
        [MaxLength(200)]
        public string resource_name { get; set; } = string.Empty;

        [Required]
        [MaxLength(4000)]
        public string description { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string contact_details { get; set; } = string.Empty;

        [Required]
        [MaxLength(75)]
        public string verification_status { get; set; } = string.Empty;
        public string availability_status { get; set; } = "Tilgjengelig nå";
        public bool archived { get; set; }
        public byte[]? image_data { get; set; }
        public string? image_content_type { get; set; }
        // Navigasjonsegenskaper
        public ProviderAccount ProviderAccount { get; set; } = null!;
        public ResourceType ResourceType { get; set; } = null!;
        public Location Location { get; set; } = null!;
    }
}
