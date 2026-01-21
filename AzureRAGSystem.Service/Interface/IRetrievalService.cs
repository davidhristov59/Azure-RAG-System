using AzureRAGSystem.Domain.DTOs.Chat;

namespace AzureRAGSystem.Service.Interface;

public interface IRetrievalService
{
    Task<ChatResponseDTO> ProcessUserQueryAsync(string userId, ChatRequestDTO chatRequestDto);
}