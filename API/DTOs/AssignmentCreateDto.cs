using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class AssignmentCreateDto
{
    [Required]
    [StringLength(24, MinimumLength = 1)]
    public required string Name { get; set; }

    [Required]
    [StringLength(150, MinimumLength = 1)]
    public required string Description { get; set; }
    
    [Range(1, 10)]
    public int MaxUsers { get; set; } = 1;
}
