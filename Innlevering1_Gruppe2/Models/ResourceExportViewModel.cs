namespace Reachout.Models;

public class ResourceExportViewModel
{
    public static readonly string[] Regions = { "Kristiansand", "Lindesnes", "Vennesla", "Lillesand", "Birkenes", "Iveland" };
    public string? Region { get; set; }
    public List<ResourceExportRow> Resources { get; set; } = new();
}

public class ResourceExportRow
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Availability { get; set; } = string.Empty;
}
