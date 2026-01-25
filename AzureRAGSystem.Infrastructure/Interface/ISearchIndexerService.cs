using Azure.Search.Documents;
using AzureRAGSystem.Domain.DomainModels;

namespace AzureRAGSystem.Infrastructure.Interface;

/*
 * This service manages the "bridge" between storage and search. It can trigger the indexer to run on-demand and remove specific document entries from the index.
 */
public interface ISearchIndexerService
{
    Task RunIndexerAsync();
    Task DeleteDocumentFromIndexAsync(string fileName);
    Task<List<DocumentModel>> SearchDocumentsAsync(string searchText, SearchOptions options);
    Task<string> GetIndexerStatusAsync();
}