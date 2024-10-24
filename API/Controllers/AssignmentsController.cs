using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class AssignmentsController(IAssignmentRepository assignmentRepository, IMapper mapper,
    IUserRepository userRepository, IGroupRepository groupRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetAssignments([FromQuery ]int groupId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var group = await groupRepository.GetGroupByIdAsync(groupId);
        if (group == null) return BadRequest("Could not find group");

        if (!await IsUserInGroup(user.Id, groupId))
            return Unauthorized();

        var assignments = await assignmentRepository.GetAssignmentsAsync(groupId);
        foreach (var assignment in assignments)
        {
            assignment.UsersAssigned = await assignmentRepository.GetUsersAssignedInAsync(assignment.Id);
        }

        return Ok(assignments);
    }

    [HttpGet("{assignmentId}")]
    public async Task<ActionResult<AssignmentDto>> GetAssignment(int assignmentId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var assignment = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        if (assignment == null) return BadRequest("Could not find assignment");

        var group = await groupRepository.GetGroupByIdAsync(assignment.GroupId);
        if (group == null) return BadRequest("Could not find group");

        if (!await IsUserInGroup(user.Id, group.Id))
            return Unauthorized();

        var result = mapper.Map<AssignmentDto>(assignment);
        result.UsersAssigned = await assignmentRepository.GetUsersAssignedInAsync(assignment.Id);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AssignmentDto>> CreateAssignment(AssignmentCreateDto 
        assignmentCreateDto, [FromQuery] int groupId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var group = await groupRepository.GetGroupByIdAsync(groupId);
        if (group == null) return BadRequest("Could not find group");

        if (!await IsUserInGroup(user.Id, groupId))
            return Unauthorized();

        var assignment = new Assignment
        {
            Name = assignmentCreateDto.Name,
            Description = assignmentCreateDto.Description,
            MaxUsers = assignmentCreateDto.MaxUsers,
            CreatedBy = user,
            Group = group
        };

        assignmentRepository.AddAssignment(assignment);

        if (await assignmentRepository.Complete())
            return Ok(mapper.Map<AssignmentDto>(assignment));
        return BadRequest("Failed to create assignment");
    }

    [HttpPut("{assignmentId}")]
    public async Task<ActionResult<AssignmentDto>> EditAssignment(int assignmentId, 
        AssignmentCreateDto assignmentEditDto)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var assignment = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        if (assignment == null) return BadRequest("Could not find assignment");

        if (assignment.CreatedById != user.Id) return Unauthorized();

        if (assignment.Name != assignmentEditDto.Name)
        {
            assignment.Name = assignmentEditDto.Name;
        }

        if (assignment.Description != assignmentEditDto.Description)
        {
            assignment.Description = assignmentEditDto.Description;
        }

        if (assignment.MaxUsers != assignmentEditDto.MaxUsers)
        {
            assignment.MaxUsers = assignmentEditDto.MaxUsers;
        }

        if (await assignmentRepository.Complete())
            return Ok(mapper.Map<AssignmentDto>(assignment));
        return BadRequest("Failed to edit assignment");
    }

    [HttpDelete("{assignmentId}")]
    public async Task<ActionResult> DeleteAssignment(int assignmentId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var assignment = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        if (assignment == null) return BadRequest("Could not find assignment");

        if (assignment.CreatedById != user.Id) return Unauthorized();

        assignmentRepository.DeleteAssignment(assignment);

        if (await assignmentRepository.Complete()) return NoContent();
        return BadRequest("Failed to delete assignment");
    }

    [HttpPost("{assignmentId}/join")]
    public async Task<ActionResult> JoinAssignment(int assignmentId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var assignment = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        if (assignment == null) return BadRequest("Could not find assignment");

        var group = await groupRepository.GetGroupByIdAsync(assignment.GroupId);
        if (group == null) return BadRequest("Could not find group");

        if (!await IsUserInGroup(user.Id, group.Id))
            return Unauthorized();

        var usersAssigned = await assignmentRepository.GetUsersAssignedInAsync(assignmentId);

        var userAssignment = await assignmentRepository.GetUserAssignmentAsync(user.Id, assignmentId);
        if (userAssignment != null)
            return BadRequest("You cannot join assignment you have already assigned in");

        if (usersAssigned.Count() >= assignment.MaxUsers)
            return BadRequest("All spots in assignment has been taken");

        assignmentRepository.AddUserToAssignment(user.Id, assignmentId);

        if (await assignmentRepository.Complete()) return NoContent();
        return BadRequest("Failed to join assignment");
    }

    [HttpPost("{assignmentId}/leave")]
    public async Task<ActionResult> LeaveAssignment(int assignmentId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var assignment = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        if (assignment == null) return BadRequest("Could not find assignment");

        var group = await groupRepository.GetGroupByIdAsync(assignment.GroupId);
        if (group == null) return BadRequest("Could not find group");

        if (!await IsUserInGroup(user.Id, group.Id))
            return Unauthorized();

        var userAssignment = await assignmentRepository.GetUserAssignmentAsync(user.Id, assignmentId);
        if (userAssignment == null)
            return BadRequest("You cannot leave assignment you have not assigned in yet");

        assignmentRepository.RemoveUserFromAssignment(userAssignment);

        if (await assignmentRepository.Complete()) return NoContent();
        return BadRequest("Failed to leave assignment");
    }

    [HttpPost("{assignmentId}/complete")]
    public async Task<ActionResult> SetAssignmentAsComplete(int assignmentId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var assignment = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        if (assignment == null) return BadRequest("Could not find assignment");

        var group = await groupRepository.GetGroupByIdAsync(assignment.GroupId);
        if (group == null) return BadRequest("Could not find group");

        if (!await IsUserInGroup(user.Id, group.Id))
            return Unauthorized();

        var userAssignment = await assignmentRepository.GetUserAssignmentAsync(user.Id, assignmentId);
        if (userAssignment == null)
            return BadRequest("You have not assigned to this assignment");

        if (userAssignment.IsCompleted == true)
            return BadRequest("You have already completed this assignment");
        
        userAssignment.IsCompleted = true;

        var usersAssigned = await assignmentRepository.GetUsersAssignedInAsync(assignmentId);
        var usersChecked = 0;
        var usersCompleted = 0;
        foreach (var userAssigned in usersAssigned)
        {
            usersChecked++;
            if (userAssigned.IsCompleted == true) usersCompleted++;
        }

        if (usersChecked == usersCompleted + 1) assignment.Completed = true;

        if (await assignmentRepository.Complete()) return NoContent();
        return BadRequest("Failed to set assignment as completed");
    }

    private async Task<bool> IsUserInGroup(int userId, int groupId)
    {
        return await groupRepository.GetUserGroupAsync(userId, groupId) != null;
    }
}
