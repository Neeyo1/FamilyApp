using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IAssignmentRepository
{
    void AddAssignment(Assignment assignment);
    void DeleteAssignment(Assignment assignment);
    Task<Assignment?> GetAssignmentByIdAsync(int assignmentId);
    Task<PagedList<AssignmentDto>> GetAssignmentsAsync(int groupId, AssignmentParams assignmentParams);
    void AddUserToAssignment(int userId, int assignmentId);
    void RemoveUserFromAssignment(UserAssignment userAssignment);
    Task<UserAssignment?> GetUserAssignmentAsync(int userId, int assignmentId);
    Task<IEnumerable<AssignedMemberDto>> GetUsersAssignedInAsync(int assignmentId);
    Task<bool> Complete();
}
