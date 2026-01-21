using System.Text.Json.Serialization;

namespace AzureRAGSystem.Domain.DomainModels;

public class ChatSession : BaseEntity 
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = "Ticket";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>(); // one Session has many interactions
}