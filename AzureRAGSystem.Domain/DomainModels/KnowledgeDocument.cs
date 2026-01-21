namespace AzureRAGSystem.Domain.DomainModels;

public class KnowledgeDocument : BaseEntity 
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; 
    public string? Category { get; set; }
    public string? SourceUrl { get; set; } // path to the document in Azure Blob Storage
    public string? ChunkId { get; set; }  // useful for mapping back to Azure AI Search
}