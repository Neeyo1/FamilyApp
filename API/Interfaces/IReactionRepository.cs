using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IReactionRepository
{
    void AddReaction(Reaction reaction);
    void DeleteReaction(Reaction reaction);
    Task<Reaction?> GetReactionAsync(int assignmentId, int userId, string name);
    Task<IEnumerable<ReactionDto>> GetReactionsAsync(int assignmentId);
    Task<bool> Complete();
}
