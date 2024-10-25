namespace API.DTOs;

public class GroupDto
{
    public int Id { get; set; }
    public required string GroupName { get; set; }
    public DateTime CreatedAt { get; set; }
    public MemberDto Owner { get; set; } = null!;
    public int MembersCount { get; set; }
    public IEnumerable<MemberDto> Members { get; set; } = [];
    public IEnumerable<AssignmentDto> Assignments { get; set; } = [];
}
