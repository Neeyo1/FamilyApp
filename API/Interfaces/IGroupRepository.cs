using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IGroupRepository
{
    void AddGroup(Group group);
    void DeleteGroup(Group group);
    Task<Group?> GetGroupByIdAsync(int groupId);
    Task<Group?> GetGroupByGroupNameAsync(string groupName);
    Task<IEnumerable<GroupDto>> GetMyGroupsAsync(int userId);
    void AddUserToGroup(int userId, int groupId);
    void RemoveUserFromGroup(UserGroup userGroup);
    Task<UserGroup?> GetUserGroupAsync(int userId, int groupId);
    Task<IEnumerable<MemberDto>> GetGroupMembersAsync(int groupId);
    Task<bool> Complete();
}
