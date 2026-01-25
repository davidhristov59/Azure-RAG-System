namespace AzureRAGSystem.Domain.DTOs.Chat;

public record ChatRequestDTO(
    Guid? SessionId,  // null for new chats
    string Message,  
    string? CategoryFilter // to narrow search to specific PDF categories
);