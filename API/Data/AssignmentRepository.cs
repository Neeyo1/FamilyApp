using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AssignmentRepository(DataContext context, IMapper mapper) : IAssignmentRepository
{
    public void AddAssignment(Assignment assignment)
    {
        context.Assignments.Add(assignment);
    }

    public void DeleteAssignment(Assignment assignment)
    {
        context.Assignments.Remove(assignment);
    }

    public async Task<Assignment?> GetAssignmentByIdAsync(int assignmentId)
    {
        return await context.Assignments.FindAsync(assignmentId);
    }

    public async Task<IEnumerable<AssignmentDto>> GetAssignmentsAsync(int groupId)
    {
        return await context.Assignments
            .Where(x => x.GroupId == groupId)
            .ProjectTo<AssignmentDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public void AddUserToAssignment(int userId, int assignmentId)
    {
        var userAssignment = new UserAssignment
        {
            UserId = userId,
            AssignmentId = assignmentId
        };
        context.UserAssignments.Add(userAssignment);
    }

    public void RemoveUserFromAssignment(UserAssignment userAssignment)
    {
        context.UserAssignments.Remove(userAssignment);
    }

    public async Task<UserAssignment?> GetUserAssignmentAsync(int userId, int assignmentId)
    {
        return await context.UserAssignments
            .Where(x => x.AssignmentId == assignmentId)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<IEnumerable<AssignedMemberDto>> GetUsersAssignedInAsync(int assignmentId)
    {
        return await context.UserAssignments
            .Where(x => x.AssignmentId == assignmentId)
            .ProjectTo<AssignedMemberDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
