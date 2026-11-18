using Reachout.Models.Entities;

namespace Reachout.Models;

public class VolunteerDirectoryEntryViewModel
{
    public VolunteerProfile Profile { get; set; } = null!;
    public IReadOnlyList<Availability> Periods { get; set; } = Array.Empty<Availability>();
}
