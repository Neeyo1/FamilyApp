using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class ReactionsController(IUserRepository userRepository, IReactionRepository reactionRepository, 
    IAssignmentRepository assignmentRepository, IGroupRepository groupRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReactionDto>>> GetReactions([FromQuery] int assignmentId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var assignment = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        if (assignment == null) return BadRequest("Could not find assignment");

        var group = await groupRepository.GetGroupByIdAsync(assignment.GroupId);
        if (group == null) return BadRequest("Could not find group");

        if (!await IsUserInGroup(user.Id, group.Id))
            return Unauthorized();

        var reactions = await reactionRepository.GetReactionsAsync(assignmentId);
        return Ok(reactions);
    }

    [HttpPost]
    public async Task<ActionResult> AddReaction([FromQuery] int assignmentId, string name)
    {
        string[] reactionNames = ["like", "dislike", "heart", "wow", "exhaust"];
        if (!reactionNames.Contains(name.ToLower()))
            return BadRequest("Wrong reaction name");

        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var assignment = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        if (assignment == null) return BadRequest("Could not find assignment");

        var group = await groupRepository.GetGroupByIdAsync(assignment.GroupId);
        if (group == null) return BadRequest("Could not find group");

        if (!await IsUserInGroup(user.Id, group.Id))
            return Unauthorized();

        if (await HasUserReacted(user.Id, assignmentId, name))
            return BadRequest("You have already reacted with this reaction");

        var reaction = new Reaction
        {
            User = user,
            Assignment = assignment,
            Name = name.ToLower()
        };

        reactionRepository.AddReaction(reaction);

        if (await reactionRepository.Complete()) return NoContent();
        return BadRequest("Failed to create reaction");
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteReaction([FromQuery] int assignmentId, string name)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Could not find user");

        var assignment = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        if (assignment == null) return BadRequest("Could not find assignment");

        var group = await groupRepository.GetGroupByIdAsync(assignment.GroupId);
        if (group == null) return BadRequest("Could not find group");

        if (!await IsUserInGroup(user.Id, group.Id))
            return Unauthorized();

        if (!await HasUserReacted(user.Id, assignmentId, name))
            return BadRequest("You have not reacted with this reaction yet");

        var reaction = await reactionRepository.GetReactionAsync(assignmentId, user.Id, name.ToLower());
        if (reaction == null)
            return BadRequest("Could not find reaction to delete");

        reactionRepository.DeleteReaction(reaction);

        if (await reactionRepository.Complete()) return NoContent();
        return BadRequest("Failed to delete reaction");
    }

    private async Task<bool> IsUserInGroup(int userId, int groupId)
    {
        return await groupRepository.GetUserGroupAsync(userId, groupId) != null;
    }

    private async Task<bool> HasUserReacted(int userId, int assignmentId, string name)
    {
        return await reactionRepository.GetReactionAsync(assignmentId, userId, name.ToLower()) != null;
    }
}
