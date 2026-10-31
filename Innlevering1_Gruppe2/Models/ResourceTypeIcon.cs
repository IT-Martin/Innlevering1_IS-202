namespace Innlevering1_Gruppe2.Models;

public static class ResourceTypeIcon
{
    public static string For(string? categoryName, string? resourceName = null)
    {
        var searchableText = $"{categoryName} {resourceName}"
            .Trim()
            .ToLowerInvariant();

        if (searchableText.Contains("drone"))
        {
            return "fa-solid fa-helicopter";
        }

        if (searchableText.Contains("traktor"))
        {
            return "fa-solid fa-tractor";
        }

        return categoryName?.Trim().ToLowerInvariant() switch
        {
            "kjøretøy" or "kjoeretoy" or "bil" => "fa-solid fa-car-side",
            "heisekran" or "kran" => "fa-solid fa-tower-observation",
            "frivillige" or "frivillig" => "fa-solid fa-people-group",
            "tilhenger" => "fa-solid fa-truck",
            "verktøy" or "verktoy" => "fa-solid fa-toolbox",
            "førstehjelp" or "forstehjelp" => "fa-solid fa-kit-medical",
            _ => "fa-solid fa-location-dot"
        };
    }
}
