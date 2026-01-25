namespace AzureRAGSystem.Domain.DTOs.Session;

public record UserSessionDTO
(
    Guid Id,
    string Title,
    DateTime CreatedAt,
    string UserId
);