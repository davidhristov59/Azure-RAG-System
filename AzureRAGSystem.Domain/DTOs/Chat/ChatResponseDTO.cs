namespace AzureRAGSystem.Domain.DTOs.Chat;

public record ChatResponseDTO(
    Guid SessionId, 
    string ResponseMessage ,
    List<SourceDTO> Sources, // list of sources used for the response
    DateTime Timestamp 
);