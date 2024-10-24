using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Assigments")]
public class Assignment
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime EndsAt { get; set; } = DateTime.UtcNow.AddDays(7); //For now until proper date picker
    public bool Completed { get; set; } = false;
    public int MaxUsers { get; set; } = 1;

    //Assigment - User(creator)
    public int CreatedById { get; set; }
    public AppUser CreatedBy { get; set; } = null!;

    //Assigment - Group
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

    //Assignment - User
    public ICollection<UserAssignment> UserAssignments { get; set; } = [];
}
