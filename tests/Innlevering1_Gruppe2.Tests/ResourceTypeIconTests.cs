using Reachout.Models;

namespace Reachout.Tests;

/* Testklasse for testing av ResourceTypeIcon. Tester ulike ressurskategorier og deres tilhørende ikoner */
public class ResourceTypeIconTests
{
    [Theory]
    [InlineData("Kjøretøy", "fa-solid fa-car-side")]
    [InlineData("Kjoeretoy", "fa-solid fa-car-side")]
    [InlineData("Heisekran", "fa-solid fa-tower-observation")]
    [InlineData("Frivillige", "fa-solid fa-people-group")]
    [InlineData("Tilhenger", "fa-solid fa-truck")]
    [InlineData("Verktøy", "fa-solid fa-toolbox")]
    [InlineData("Førstehjelp", "fa-solid fa-kit-medical")]
    [InlineData("Ukjent", "fa-solid fa-location-dot")]
    public void For_ReturnsIconForResourceCategory(string category, string expectedIcon)
    {
        Assert.Equal(expectedIcon, ResourceTypeIcon.For(category));
    }

    [Theory]
    [InlineData("Kjøretøy", "Traktor", "fa-solid fa-tractor")]
    [InlineData("Ukjent", "Drone", "fa-solid fa-helicopter")]
    [InlineData(null, "Drone til søk", "fa-solid fa-helicopter")]
    public void For_PrioritizesSpecificResourceName(
        string? category,
        string resourceName,
        string expectedIcon)
    {
        Assert.Equal(expectedIcon, ResourceTypeIcon.For(category, resourceName));
    }

    [Fact]
    public void For_IsCaseInsensitiveAndTrimsWhitespace()
    {
        Assert.Equal("fa-solid fa-tractor", ResourceTypeIcon.For(" KJØRETØY ", " TRaktor "));
    }
}
