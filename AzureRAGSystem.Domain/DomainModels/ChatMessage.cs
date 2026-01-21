namespace AzureRAGSystem.Domain.DomainModels;

public class ChatMessage : BaseEntity
{
    public Guid SessionId { get; set; }
    public string Role { get; set; } // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
    
    public List<string>? Citations { get; set; }  // store which documents were used to generate this specific answer
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}