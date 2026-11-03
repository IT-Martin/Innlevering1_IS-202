using System.ComponentModel.DataAnnotations;

namespace Innlevering1_Gruppe2.Models.ViewModels.Resource
{
    public class ResourceViewModel
    {


        [Required(ErrorMessage = "Du må skrive ressursnavn")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ressurs beskrivelse trengs")]
        public string Description { get; set; } = string.Empty;

        public List<string> AvailableTypes { get; set; } = new List<string> { "Kjøretøy", "Heisekran", "Frivillige", "Tilhenger" };

        public List<string> SelectedTypes { get; set; } = new List<string>();

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public int ProviderAccountId { get; set; }
        public int ResourceTypeId { get; set; }
        public int LocationId { get; set; }
        public string? ContactDetails { get; set; }



    }
}
