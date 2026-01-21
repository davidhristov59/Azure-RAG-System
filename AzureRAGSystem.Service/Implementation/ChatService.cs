using System.Net.Http.Json;
using AzureRAGSystem.Domain.DTOs.Chat;
using AzureRAGSystem.Service.Interface;

namespace AzureRAGSystem.Service.Implementation;

public class ChatService : IChatService
{
    private readonly HttpClient _httpClient;
    
    public ChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ChatResponseDTO?> AskQuestionAsync(string message, Guid? sessionId = null)
    {
        var request = new ChatRequestDTO(sessionId, message, null);

        var response = await _httpClient.PostAsJsonAsync("api/Chat/query", request);
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ChatResponseDTO>(); // deserializes the JSON response to ChatResponseDTO
        }
        return null;
    }
}