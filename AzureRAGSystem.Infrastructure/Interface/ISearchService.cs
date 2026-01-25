using AzureRAGSystem.Domain.DomainModels;
using AzureRAGSystem.Domain.DTOs.Search;

namespace AzureRAGSystem.Infrastructure.Interface;

/**
    Interface for search services that interaction with the knowledge base.
    Connects to Azure AI Search, fetches chunks, and maps them to the object KnowledgeDocument.
*/
public interface ISearchService
{
    Task<List<KnowledgeDocument>> SearchKnowledgeBaseAsync(SearchRequestDTO searchRequestDto);
}