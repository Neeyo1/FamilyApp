using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class GroupRepository(DataContext context, IMapper mapper) : IGroupRepository
{
    public void AddGroup(Group group)
    {
        context.Groups.Add(group);
    }

    public void DeleteGroup(Group group)
    {
        context.Groups.Remove(group);
    }

    public async Task<Group?> GetGroupByIdAsync(int groupId)
    {
        return await context.Groups
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.Id == groupId);
    }

    public async Task<Group?> GetGroupByGroupNameAsync(string groupName)
    {
        return await context.Groups
            .SingleOrDefaultAsync(x => x.GroupName == groupName);
    }

    public async Task<IEnumerable<GroupDto>> GetMyGroupsAsync(int userId)
    {  
        return await context.UserGroups
            .Where(x => x.UserId == userId)
            .Select(x => x.Group)
            .ProjectTo<GroupDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public void AddUserToGroup(int userId, int groupId)
    {
        var userGroup = new UserGroup
        {
            UserId = userId,
            GroupId = groupId
        };
        context.UserGroups.Add(userGroup);
    }

    public void RemoveUserFromGroup(UserGroup userGroup)
    {
        context.UserGroups.Remove(userGroup);
    }

    public async Task<UserGroup?> GetUserGroupAsync(int userId, int groupId)
    {
        return await context.UserGroups
            .Where(x => x.GroupId == groupId)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<IEnumerable<MemberDto>> GetGroupMembersAsync(int groupId)
    {
        return await context.UserGroups
            .Where(x => x.GroupId == groupId)
            .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
