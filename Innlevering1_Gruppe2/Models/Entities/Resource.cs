using System.ComponentModel.DataAnnotations;

namespace Innlevering1_Gruppe2.Models.Entities
{
    public class Resource
    {
        public int resource_id { get; set; }              // PK
        public int account_id { get; set; }                // FK til eier
        public int resource_type_id { get; set; }          // FK
        public int location_id { get; set; }               // FK

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

        // Navigasjonsegenskaper
        public Account Account { get; set; } = null!;
        public ResourceType ResourceType { get; set; } = null!;
        public Location Location { get; set; } = null!;
    }
}