using AzureRAGSystem.Domain.DomainModels;
using AzureRAGSystem.Domain.DTOs.Chat;
using AzureRAGSystem.Domain.DTOs.Search;
using AzureRAGSystem.Infrastructure.Interface;
using AzureRAGSystem.Service.Interface;
using AzureRAGSystem.Repository.Interface;

namespace AzureRAGSystem.Service.Implementation;

public class RetrievalService : IRetrievalService
{
    private readonly IAzureOpenAIService _azureOpenAIService;
    private readonly ISearchService _searchService;
    private readonly ICosmosdbRepository _cosmosDbRepository;
    
    public RetrievalService(
        IAzureOpenAIService azureOpenAIService,
        ISearchService searchService,
        ICosmosdbRepository cosmosDbRepository)
    {
        _azureOpenAIService = azureOpenAIService;
        _searchService = searchService;
        _cosmosDbRepository = cosmosDbRepository;
    }
    
    public async Task<ChatResponseDTO> ProcessUserQueryAsync(string userId, ChatRequestDTO request)
    /*
     * Orchestrates the retrieval and response generation process.
     */
    {
        ChatSession session = null;
        if (request.SessionId.HasValue && request.SessionId != Guid.Empty)
        {
            session = await _cosmosDbRepository.GetSessionByIdAsync(request.SessionId.ToString(), userId);
        }

        if (session == null)
        {
            session = await InitializeNewSession(userId);
        }

        // Retrieval
        var searchResults = await _searchService.SearchKnowledgeBaseAsync(new SearchRequestDTO(
            Query: request.Message,
            CategoryFilter: request.CategoryFilter,
            MaxResults: 2
        ));
        
        Console.WriteLine($"Found {searchResults.Count} relevant document chunks.");

        // Generation
        var aiAnswer = await _azureOpenAIService.GenerateResponseAsync(request.Message, searchResults);

        // Update session with new interaction
        UpdateSessionHistory(session, request.Message , aiAnswer, searchResults);
        
        if (session.Messages.Count <= 2) // added the first pair of messages
        {
            session.Title = request.Message.Length > 30 
                ? request.Message.Substring(0, 30) + "..." 
                : request.Message;
        }
        
        await _cosmosDbRepository.SaveSessionAsync(session);

        return new ChatResponseDTO(
            SessionId : session.Id,
            ResponseMessage : aiAnswer ?? "No answer could be generated.",
            Sources: searchResults.Select(r => new SourceDTO(
                r.Title ?? "Unknown PDF", 
                r.SourceUrl ?? "N/A", 
                r.ChunkId ?? "0"
            )).ToList(),
            Timestamp : DateTime.UtcNow
        );
    }
    
    private async Task<ChatSession> InitializeNewSession(string userId)
    {
        var session = new ChatSession()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = "New Chat Session",
            CreatedAt = DateTime.UtcNow,
            Messages = new List<ChatMessage>()
        };
        // await _cosmosDbRepository.CreateSessionAsync(session);
        return session;
    }

    private void UpdateSessionHistory(
        ChatSession chatSession, 
        string userMessage, 
        string aiResponse,
        List<KnowledgeDocument> retrievedDocs)
    {
        // User message
        chatSession.Messages.Add(new ChatMessage()
        {
            Id = Guid.NewGuid(),
            SessionId = chatSession.Id,
            Content = userMessage,
            Role = "user",
            Timestamp = DateTime.UtcNow,
            Citations = retrievedDocs.Select(doc => doc.Id.ToString()).ToList(),
        });
        
        // AI response message
        chatSession.Messages.Add(new ChatMessage()
        {
            Id = Guid.NewGuid(),
            SessionId = chatSession.Id,
            Content = aiResponse ?? "No answer could be generated.", // Ensure DB isn't saving null
            Role = "assistant",
            Citations = retrievedDocs.Select(doc => doc.Id.ToString()).ToList(),
            Timestamp = DateTime.UtcNow,
        });

    }
}
