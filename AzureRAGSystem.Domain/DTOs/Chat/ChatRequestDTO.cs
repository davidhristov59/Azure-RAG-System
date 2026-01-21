namespace AzureRAGSystem.Domain.DTOs.Chat;

public record ChatRequestDTO(
    Guid? SessionId, 
    string Message, 
    string? CategoryFilter // to narrow search to specific PDF categories
);