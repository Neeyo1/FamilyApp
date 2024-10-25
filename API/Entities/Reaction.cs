using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Reactions")]
public class Reaction
{
    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public int AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;
    public required string Name { get; set; }
}
