using System.ComponentModel.DataAnnotations;
using Reachout.Models.Entities;

namespace Reachout.Models;
public class ResourceDetailsViewModel
{
    public Resource Resource { get; set; } = null!;
    public List<Availability> Periods { get; set; } = new();
    public List<ResourceNeed> Needs { get; set; } = new();
    public bool CanEdit { get; set; }
}
