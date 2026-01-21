using AzureRAGSystem.Domain.DTOs.Chat;

namespace AzureRAGSystem.Service.Interface;

public interface IChatService
{
    Task<ChatResponseDTO?> AskQuestionAsync(string message, Guid? sessionId);
}