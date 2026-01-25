using AzureRAGSystem.Service.Interface;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;
using AzureRAGSystem.Domain.DomainModels;

namespace AzureRAGSystem.Service.Implementation;

public class DocumentService : IDocumentService
{
    private readonly HttpClient _httpClient;
    private readonly ChatService _chatService;

    public DocumentService(HttpClient httpClient, IChatService chatService)
    {
        _httpClient = httpClient;
        _chatService = (ChatService)chatService; 
    }

    public async Task<string> UploadDocumentAsync(IBrowserFile file, string category)
    {
        AddAuthHeaders();
        using var content = new MultipartFormDataContent();
        
        var fileStream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
        var streamContent = new StreamContent(fileStream);
        
        content.Add(streamContent, "file", file.Name);
        content.Add(new StringContent(category), "category");

        var response = await _httpClient.PostAsync("api/Documents/upload", content);
        return response.IsSuccessStatusCode ? "Success" : "Upload Failed";
    }

    public async Task<List<DocumentModel>> GetDocumentsAsync()
    {
        AddAuthHeaders();
        var docs = await _httpClient.GetFromJsonAsync<List<DocumentModel>>("api/Documents");
    
        Console.WriteLine($"DEBUG: UI received {docs?.Count ?? 0} documents.");
    
        return docs ?? new List<DocumentModel>();
    }
    
    public async Task<bool> DeleteDocumentAsync(string fileName)
    {
        AddAuthHeaders();
        var response = await _httpClient.DeleteAsync($"api/Documents/{fileName}");
        return response.IsSuccessStatusCode;
    }

    public async Task<string> GetDocumentPreviewUrlAsync(string fileName)
    {
        AddAuthHeaders();
        var response = await _httpClient.GetFromJsonAsync<PreviewUrlResponse>($"api/Documents/preview/{fileName}");
        return response?.Url ?? string.Empty;
    }

    private void AddAuthHeaders()
    {
        _httpClient.DefaultRequestHeaders.Remove("X-User-Id");
        _httpClient.DefaultRequestHeaders.Add("X-User-Id", _chatService.CurrentUserId);
    }

    private class PreviewUrlResponse
    {
        public string Url { get; set; }
    }
}