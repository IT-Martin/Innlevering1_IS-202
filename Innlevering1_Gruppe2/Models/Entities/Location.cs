using System.ComponentModel.DataAnnotations;

namespace Reachout.Models.Entities
{
    public class Location
    {
        [Key]
        public int location_id { get; set; }

        [Required]
        [MaxLength(500)]
        public string address_or_area { get; set; } = string.Empty;
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        public ICollection<Resource> Resources { get; set; } = new List<Resource>();
    }
}
