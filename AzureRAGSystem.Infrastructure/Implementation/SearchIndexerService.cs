using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using AzureRAGSystem.Domain.DomainModels;
using AzureRAGSystem.Infrastructure.Interface;

namespace AzureRAGSystem.Infrastructure.Implementation;

public class SearchIndexerService : ISearchIndexerService
{
    private readonly SearchIndexerClient _indexerClient;
    private readonly SearchClient _searchClient;
    private readonly string _indexerName = "is-docs-indexer";

    public SearchIndexerService(string endpoint, string apiKey, string indexName)
    {
        var credential = new AzureKeyCredential(apiKey);
        var uri = new Uri(endpoint);
        
        _indexerClient = new SearchIndexerClient(uri, credential);
        _searchClient = new SearchClient(uri, indexName, credential);
    }

    public async Task RunIndexerAsync()
    {
        // Triggers the indexer immediately
        await _indexerClient.RunIndexerAsync(_indexerName);
    }

    public async Task DeleteDocumentFromIndexAsync(string documentId)
    {
        await _searchClient.DeleteDocumentsAsync("id", new[] { documentId });
    }
    
    public async Task<List<DocumentModel>> SearchDocumentsAsync(string searchText, SearchOptions options)
    {
        var results = await _searchClient.SearchAsync<DocumentModel>(searchText, options);
        var docs = new List<DocumentModel>();

        await foreach (var result in results.Value.GetResultsAsync())
        {
            docs.Add(result.Document);
        }

        return docs;
    }

    public async Task<string> GetIndexerStatusAsync()
    {
        try
        {
            var status =  await _indexerClient.GetIndexerStatusAsync(_indexerName);

            return status.Value.Status.ToString();
        }
        catch (Exception e)
        {
            return "Error";
        }
    }
}