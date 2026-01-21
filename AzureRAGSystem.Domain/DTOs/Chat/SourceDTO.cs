using Newtonsoft.Json;

namespace AzureRAGSystem.Domain.DTOs.Chat;

public record SourceDTO(
    [JsonProperty(PropertyName = "title")]
    string Title,
    [JsonProperty(PropertyName = "sourceUrl")]
    string SourceDocument, 
    [JsonProperty(PropertyName = "chunkId")]
    string? ChunkId
);