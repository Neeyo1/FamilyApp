using API.DTOs;
using API.Entities;
using API.Helpers;
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

    public async Task<PagedList<GroupDto>> GetMyGroupsAsync(int userId, GroupParams groupParams)
    {  
        var query = context.UserGroups
            .Where(x => x.UserId == userId)
            .Select(x => x.Group)
            .AsQueryable();

        if (groupParams.GroupName != null)
        {
            query = query.Where(x => x.GroupName == groupParams.GroupName);
        }
        if (groupParams.Owner != null)
        {
            query = query.Where(x => x.Owner.KnownAs == groupParams.Owner);
        }
        query = query.Where(x => x.MembersCount >= groupParams.MinMembers 
            && x.MembersCount <= groupParams.MaxMembers);

        query = groupParams.OrderBy switch
        {
            "oldest" => query.OrderBy(x => x.CreatedAt),
            "newest" => query.OrderByDescending(x => x.CreatedAt),
            "members" => query.OrderBy(x => x.MembersCount),
            "membersDesc" => query.OrderByDescending(x => x.MembersCount),
            _ => query.OrderByDescending(x => x.CreatedAt),
        };

        return await PagedList<GroupDto>.CreateAsync(
            query.ProjectTo<GroupDto>(mapper.ConfigurationProvider), 
            groupParams.PageNumber, groupParams.PageSize);
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
