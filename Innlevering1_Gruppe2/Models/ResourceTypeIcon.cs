namespace Reachout.Models;
public static class ResourceTypeIcon
{
    public static string CategoryClass(string? categoryName) => categoryName?.Trim().ToLowerInvariant() switch
    {
        "vann" => "map-category-water",
        "strøm" => "map-category-power",
        "kjøretøy" or "kjoeretoy" or "bil" => "map-category-vehicle",
        "utstyr" or "verktøy" or "verktoy" or "heisekran" or "kran" or "tilhenger" => "map-category-equipment",
        "lokaler" => "map-category-building",
        _ => "map-category-other"
    };

    public static string For(string? categoryName, string? resourceName = null)
    {
        var category = categoryName?.Trim().ToLowerInvariant() ?? "";
        var name = resourceName?.Trim().ToLowerInvariant() ?? "";

// Traktorikon kun innenfor kjøretøy
        if ((category == "kjøretøy" || category == "kjoeretoy" || category == "bil")
            && name.Contains("traktor"))
        {
            return "fa-solid fa-tractor";
        }

// Droneikon kun innenfor utstyr
        if ((category == "utstyr" || category == "verktøy" || category == "verktoy")
            && name.Contains("drone"))
        {
            return "fa-solid fa-helicopter";
        }

        return categoryName?.Trim().ToLowerInvariant() switch
        {
            "kjøretøy" or "kjoeretoy" or "bil" => "fa-solid fa-car-side",
            "heisekran" or "kran" => "fa-solid fa-tower-observation",
            "frivillige" or "frivillig" => "fa-solid fa-people-group",
            "tilhenger" => "fa-solid fa-truck",
            "lokaler" => "fa-solid fa-building",
            "strøm" => "fa-solid fa-bolt",
            "vann" => "fa-solid fa-droplet",
            "utstyr" or "verktøy" or "verktoy" => "fa-solid fa-toolbox",
            "førstehjelp" or "forstehjelp" => "fa-solid fa-kit-medical",
            _ => "fa-solid fa-location-dot"
        };
    }
}
