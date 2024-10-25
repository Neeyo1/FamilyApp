using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class GroupsController(IGroupRepository groupRepository, IUserRepository userRepository, 
    IAssignmentRepository assignmentRepository, IMapper mapper) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroupDto>>> GetMyGroups()
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var groups = await groupRepository.GetMyGroupsAsync(user.Id);
        return Ok(groups);
    }

    [HttpGet("{groupId}")]
    public async Task<ActionResult<GroupDto>> GetGroup(int groupId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var group = await groupRepository.GetGroupByIdAsync(groupId);
        if (group == null) return BadRequest("Group does not exist");

        if(!await IsUserInGroup(user.Id, groupId))
            return Unauthorized();

        var result = mapper.Map<GroupDto>(group);
        result.Members = await groupRepository.GetGroupMembersAsync(groupId);
        result.Assignments = await assignmentRepository.GetAssignmentsAsync(groupId);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<GroupDto>> CreateGroup(GroupCreateDto groupCreateDto)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");
        
        var owner = await userRepository.GetUserByUsernameAsync(groupCreateDto.Owner.ToLower());
        if (owner == null) return BadRequest("Could not find user");

        if (await groupRepository.GetGroupByGroupNameAsync(groupCreateDto.GroupName) != null)
        {
            return BadRequest("Group with this name already exists");
        }

        var group = new Group
        {
            GroupName = groupCreateDto.GroupName,
            Owner = owner
        };

        groupRepository.AddGroup(group);

        if (!await groupRepository.Complete()) return BadRequest("Failed to create group");

        groupRepository.AddUserToGroup(owner.Id, group.Id);
        group.MembersCount++;

        if (await groupRepository.Complete()) return Ok(mapper.Map<GroupDto>(group));
        return BadRequest("Failed to add owner to group");
    }

    [HttpPut("{groupId}")]
    public async Task<ActionResult<GroupDto>> EditGroup(GroupCreateDto groupEditDto, int groupId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var group = await groupRepository.GetGroupByIdAsync(groupId);
        if (group == null) return BadRequest("Could not find group");

        if (group.OwnerId != user.Id) return Unauthorized();

        if (group.GroupName != groupEditDto.GroupName)
        {
            var otherGroup = await groupRepository.GetGroupByGroupNameAsync(groupEditDto.GroupName);
            if (otherGroup == null) group.GroupName = groupEditDto.GroupName;
        }

        if (group.Owner.KnownAs != groupEditDto.Owner)
        {
            var newOwner = await userRepository.GetUserByKnownAsAsync(groupEditDto.Owner);
            if (newOwner == null) return BadRequest("New owner user does not exist");

            var userGroup = await groupRepository.GetUserGroupAsync(newOwner.Id, groupId);
            if (userGroup == null) return BadRequest("New owner is not member of group");

            group.Owner = newOwner;
        }

        if (await groupRepository.Complete()) return Ok(mapper.Map<GroupDto>(group));
        return BadRequest("Failed to edit group");
    }

    [HttpDelete("{groupId}")]
    public async Task<ActionResult> DeleteGroup(int groupId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var group = await groupRepository.GetGroupByIdAsync(groupId);
        if (group == null) return BadRequest("Could not find group");

        if (group.OwnerId != user.Id) return Unauthorized();
        
        groupRepository.DeleteGroup(group);

        if (await groupRepository.Complete()) return NoContent();
        return BadRequest("Failed to delete group");
    }

    [HttpGet("{groupId}/members")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembersForGroup(int groupId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var group = await groupRepository.GetGroupByIdAsync(groupId);
        if (group == null) return BadRequest("Could not find group");

        if(!await IsUserInGroup(user.Id, groupId)) return Unauthorized();

        var members = await groupRepository.GetGroupMembersAsync(groupId);
        return Ok(members);
    }

    [HttpPost("{groupId}/members/add")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> AddMember(int groupId, [FromQuery]string userKnownAs)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var userToEdit = await userRepository.GetUserByKnownAsAsync(userKnownAs);
        if (userToEdit == null) return BadRequest("Could not find user to add");

        var group = await groupRepository.GetGroupByIdAsync(groupId);
        if (group == null) return BadRequest("Could not find group");

        if (group.OwnerId != user.Id) return Unauthorized();

        if (await IsUserInGroup(userToEdit.Id, groupId))
            return BadRequest("User is already a member of this group");

        groupRepository.AddUserToGroup(userToEdit.Id, groupId);
        group.MembersCount++;

        if (await groupRepository.Complete()) return Ok(mapper.Map<MemberDto>(userToEdit));
        return BadRequest("Failed to add member");
    }

    [HttpPost("{groupId}/members/remove")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> RemoveMember(int groupId, [FromQuery]string userKnownAs)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var userToEdit = await userRepository.GetUserByKnownAsAsync(userKnownAs);
        if (userToEdit == null) return BadRequest("Could not find user to add");

        var group = await groupRepository.GetGroupByIdAsync(groupId);
        if (group == null) return BadRequest("Could not find group");

        if (group.OwnerId == userToEdit.Id)
            return BadRequest("You cannot remove yourself from group");

        if (group.OwnerId != user.Id) return Unauthorized();

        var userGroup = await groupRepository.GetUserGroupAsync(userToEdit.Id, groupId);
        if (userGroup == null) return BadRequest("User is not member of this group");

        groupRepository.RemoveUserFromGroup(userGroup);
        group.MembersCount--;

        if (await groupRepository.Complete()) return NoContent();
        return BadRequest("Failed to delete member");
    }

    [HttpPost("{groupId}/members/leave")]
    public async Task<ActionResult> LeaveGroup(int groupId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var group = await groupRepository.GetGroupByIdAsync(groupId);
        if (group == null) return BadRequest("Could not find group");

        if (group.OwnerId == user.Id) return BadRequest("Group owner cannot leave group");

        var userGroup = await groupRepository.GetUserGroupAsync(user.Id, groupId);
        if (userGroup == null) return BadRequest("You are not member of this group");

        groupRepository.RemoveUserFromGroup(userGroup);
        group.MembersCount--;

        if (await groupRepository.Complete()) return NoContent();
        return BadRequest("Failed to edit member");
    }

    private async Task<bool> IsUserInGroup(int userId, int groupId)
    {
        return await groupRepository.GetUserGroupAsync(userId, groupId) != null;
    }
}
