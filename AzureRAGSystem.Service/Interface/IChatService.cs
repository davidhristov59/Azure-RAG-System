using AzureRAGSystem.Domain.DomainModels;
using AzureRAGSystem.Domain.DTOs.Chat;
using AzureRAGSystem.Domain.DTOs.Session;

namespace AzureRAGSystem.Service.Interface;

public interface IChatService
{
    Task<ChatResponseDTO?> AskQuestionAsync(string message, Guid? sessionId);
    Task<List<UserSessionDTO>> GetUserSessionsAsync();
    Task<List<ChatMessage>> GetChatHistoryAsync(Guid sessionId);
}