using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IAssignmentRepository
{
    void AddAssignment(Assignment assignment);
    void DeleteAssignment(Assignment assignment);
    Task<Assignment?> GetAssignmentByIdAsync(int assignmentId);
    Task<IEnumerable<AssignmentDto>> GetAssignmentsAsync(int groupId);
    void AddUserToAssignment(int userId, int assignmentId);
    void RemoveUserFromAssignment(UserAssignment userAssignment);
    Task<UserAssignment?> GetUserAssignmentAsync(int userId, int assignmentId);
    Task<IEnumerable<AssignedMemberDto>> GetUsersAssignedInAsync(int assignmentId);
    Task<bool> Complete();
}
