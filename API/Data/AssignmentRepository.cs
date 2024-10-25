using API.DTOs;
using API.Entities;
using API.Helpers;
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

    public async Task<PagedList<AssignmentDto>> GetAssignmentsAsync(int groupId,
        AssignmentParams assignmentParams)
    {
        var query = context.Assignments
            .Where(x => x.GroupId == groupId)
            .AsQueryable();

        if (assignmentParams.Name != null)
        {
            query = query.Where(x => x.Name == assignmentParams.Name);
        }
        if (assignmentParams.CreatedBy != null)
        {
            query = query.Where(x => x.CreatedBy.KnownAs == assignmentParams.CreatedBy);
        }
        query = query.Where(x => x.MaxUsers >= assignmentParams.MinUsers 
            && x.MaxUsers <= assignmentParams.MaxUsers);

        query = assignmentParams.Status switch
        {
            "completed" => query.Where(x => x.Completed == true),
            "open" => query.Where(x => x.Completed == false),
            _ => query,
        };

        query = assignmentParams.OrderBy switch
        {
            "oldest" => query.OrderBy(x => x.CreatedAt),
            "newest" => query.OrderByDescending(x => x.CreatedAt),
            "endsSoon" => query.Where(x => x.EndsAt > DateTime.UtcNow)
                               .OrderBy(x => x.EndsAt),
            "endsSoonDesc" => query.Where(x => x.EndsAt > DateTime.UtcNow)
                                   .OrderByDescending(x => x.EndsAt),
            "members" => query.OrderBy(x => x.MaxUsers),
            "membersDesc" => query.OrderByDescending(x => x.MaxUsers),
            _ => query.OrderBy(x => x.CreatedAt),
        };

        return await PagedList<AssignmentDto>.CreateAsync(
            query.ProjectTo<AssignmentDto>(mapper.ConfigurationProvider), 
            assignmentParams.PageNumber, assignmentParams.PageSize);
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
