namespace AzureRAGSystem.Domain.DTOs.Search;

public record SearchRequestDTO(
    string Query,
    string? CategoryFilter = null,
    int MaxResults = 5
);