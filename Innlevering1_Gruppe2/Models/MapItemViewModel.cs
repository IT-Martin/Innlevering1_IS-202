namespace Reachout.Models
{
    // Viser ressurser og behov i kartet uten å sende databaseobjekter til nettleseren.
    public class MapItemViewModel
    {
        public string Kind { get; set; } = "Ressurs";
        public string Place { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TypeIcon { get; set; } = "fa-solid fa-location-dot";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
