using Azure.Search.Documents.Indexes;

namespace AzureRAGSystem.Domain.DomainModels;

using System.Text.Json.Serialization;

public class DocumentModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("sourceUrl")]
    public string SourceUrl { get; set; }

    [JsonPropertyName("userId")]
    public string UserId { get; set; }

    [JsonIgnore]
    public string FileName => Title ?? "Unknown File";
}