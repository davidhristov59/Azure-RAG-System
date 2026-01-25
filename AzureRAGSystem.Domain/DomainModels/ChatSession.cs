using Newtonsoft.Json;

namespace AzureRAGSystem.Domain.DomainModels;

public class ChatSession : BaseEntity 
{
    [JsonProperty("userId")]
    public string UserId { get; set; } = string.Empty;
    
    [JsonProperty("title")]
    public string Title { get; set; } = "Ticket";
    
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [JsonProperty("messages")]
    public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>(); // one Session has many interactions
}