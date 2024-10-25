namespace API.DTOs;

public class AssignmentDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime EndsAt { get; set; }
    public bool Completed { get; set; }
    public int MaxUsers { get; set; }
    public MemberDto CreatedBy { get; set; } = null!;
    public IEnumerable<AssignedMemberDto> UsersAssigned { get; set; } = [];
    public IEnumerable<ReactionDto> Reactions { get; set; } = [];
}
