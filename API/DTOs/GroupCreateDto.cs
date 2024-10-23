using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class GroupCreateDto
{
    [Required]
    [StringLength(24, MinimumLength = 3)]
    public required string GroupName { get; set; }

    [Required] public required string Owner { get; set; }
}
