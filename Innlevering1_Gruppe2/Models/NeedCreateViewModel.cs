using System.ComponentModel.DataAnnotations;

namespace Innlevering1_Gruppe2.Models;

public class NeedCreateViewModel
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int? ResourceTypeId { get; set; }

    [Required, MaxLength(200)]
    public string Region { get; set; } = string.Empty;
}