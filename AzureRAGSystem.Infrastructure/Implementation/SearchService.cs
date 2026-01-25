using Azure;
using AzureRAGSystem.Domain.DomainModels;
using AzureRAGSystem.Domain.DTOs.Search;
using AzureRAGSystem.Infrastructure.Interface;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;

namespace AzureRAGSystem.Infrastructure.Implementation;

public class SearchService : ISearchService
{
    private readonly SearchClient _searchClient;
    
    public SearchService(IConfiguration configuration)
    {
        var endpoint = configuration["AzureSearch:Endpoint"];
        var indexName = configuration["AzureSearch:IndexName"];
        var apiKey = configuration["AzureSearch:ApiKey"];

        _searchClient = new SearchClient(endpoint: new Uri(endpoint), indexName: indexName, new AzureKeyCredential(apiKey));
    }
    
    public async Task<List<KnowledgeDocument>> SearchKnowledgeBaseAsync(SearchRequestDTO request) 
    /*
     * RETRIEVAL 
     * This method performs a search against the knowledge base using the provided search request DTO
     * and returns a list of relevant KnowledgeDocument objects from the configured Azure Search index 
     */
    {
        
        // 1. Safety check for the incoming request object
        if (request == null) return new List<KnowledgeDocument>();

        var options = new SearchOptions
        {
            Size = request.MaxResults > 0 ? request.MaxResults : 5,
            Select = { "content", "title", "sourceUrl", "category", "chunkId" }
        };

        // 2. Safe filter building: only add if the string is NOT null
        if (!string.IsNullOrWhiteSpace(request.CategoryFilter))
        {
            options.Filter = $"category eq '{request.CategoryFilter}'";
        }

        var response = await _searchClient.SearchAsync<SearchDocument>(request.Query, options);
        var results = new List<KnowledgeDocument>();

        await foreach (var result in response.Value.GetResultsAsync())
        {
            results.Add(new KnowledgeDocument
            {
                Content = result.Document.ContainsKey("content") ? result.Document["content"]?.ToString() ?? "" : "",
                Title = result.Document.ContainsKey("title") ? result.Document["title"]?.ToString() ?? "Untitled" : "Untitled",
                SourceUrl = result.Document.ContainsKey("sourceUrl") ? result.Document["sourceUrl"]?.ToString() ?? "" : "",
                Category = result.Document.ContainsKey("category") ? result.Document["category"]?.ToString() ?? "" : "",
                ChunkId = result.Document.ContainsKey("chunkId") ? result.Document["chunkId"]?.ToString() ?? "0" : "0"
            });
        }

        return results;
    }
}