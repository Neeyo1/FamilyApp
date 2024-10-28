namespace API.DTOs;

public class ReactionDto
{
    public required string Name { get; set; }
    public IEnumerable<MemberDto> Users { get; set; } = [];
}
