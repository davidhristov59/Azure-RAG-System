using AzureRAGSystem.Domain.DomainModels;

namespace AzureRAGSystem.Repository.Interface;

public interface ICosmosdbRepository
{
    Task<ChatSession> GetSessionByIdAsync(string sessionId, string userId);
    Task<List<ChatSession>> GetUserSessionsAsync(string userId);
    Task CreateSessionAsync(ChatSession session);
    Task SaveSessionAsync(ChatSession session);
}