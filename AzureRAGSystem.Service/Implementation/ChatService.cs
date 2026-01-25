using System.Net.Http.Json;
using AzureRAGSystem.Domain.DomainModels;
using AzureRAGSystem.Domain.DTOs.Chat;
using AzureRAGSystem.Domain.DTOs.Session;
using AzureRAGSystem.Service.Interface;

namespace AzureRAGSystem.Service.Implementation;

public class ChatService : IChatService
{
    private readonly HttpClient _httpClient;
    public string CurrentUserId { get; set; } = "david123";
    
    public ChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ChatResponseDTO?> AskQuestionAsync(string message, Guid? sessionId = null)
    {
        var request = new ChatRequestDTO(sessionId, message, null);

        AddUserHeader();
        var response = await _httpClient.PostAsJsonAsync("api/Chat/query", request);
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ChatResponseDTO>(); // deserializes the JSON response to ChatResponseDTO
        }
        return null;
    }

    public async Task<List<UserSessionDTO>> GetUserSessionsAsync()
    {
        AddUserHeader();
        var response = await _httpClient.GetAsync("api/Chat/sessions");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<UserSessionDTO>>() ??
                   new List<UserSessionDTO>();
        }
        return new List<UserSessionDTO>();
    }

    public async Task<List<ChatMessage>> GetChatHistoryAsync(Guid sessionId)
    {
        AddUserHeader();
        var response = await _httpClient.GetAsync($"api/Chat/history/{sessionId}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<ChatMessage>>() ?? new List<ChatMessage>();
        }
        return null;
    }
    
    private void AddUserHeader()
    {
        _httpClient.DefaultRequestHeaders.Remove("X-User-Id");
        _httpClient.DefaultRequestHeaders.Add("X-User-Id", CurrentUserId);
    }
}