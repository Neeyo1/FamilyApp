using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ReactionRepository(DataContext context, IMapper mapper) : IReactionRepository
{
    public void AddReaction(Reaction reaction)
    {
        context.Reactions.Add(reaction);
    }

    public void DeleteReaction(Reaction reaction)
    {
        context.Reactions.Remove(reaction);
    }

    public async Task<Reaction?> GetReactionAsync(int assignmentId, int userId, string name)
    {
        return await context.Reactions
            .Where(x => x.AssignmentId == assignmentId && x.UserId == userId)
            .FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<IEnumerable<ReactionDto>> GetReactionsAsync(int assignmentId)
    {
        return await context.Reactions
            .Where(x => x.AssignmentId == assignmentId)
            .GroupBy(x => x.Name)
            .ProjectTo<ReactionDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
