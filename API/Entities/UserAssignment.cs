namespace API.Entities;

public class UserAssignment
{
    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public int AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;
    public bool IsCompleted { get; set; } = false;
}
