using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IGroupRepository
{
    void AddGroup(Group group);
    void DeleteGroup(Group group);
    Task<Group?> GetGroupByIdAsync(int groupId);
    Task<Group?> GetGroupByGroupNameAsync(string groupName);
    Task<PagedList<GroupDto>> GetMyGroupsAsync(int userId, GroupParams groupParams);
    void AddUserToGroup(int userId, int groupId);
    void RemoveUserFromGroup(UserGroup userGroup);
    Task<UserGroup?> GetUserGroupAsync(int userId, int groupId);
    Task<IEnumerable<MemberDto>> GetGroupMembersAsync(int groupId);
    Task<bool> Complete();
}
