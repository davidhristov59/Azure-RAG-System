using AzureRAGSystem.Domain.DomainModels;

namespace AzureRAGSystem.Infrastructure.Interface;

public interface IAzureOpenAIService
{
    Task<string> GenerateResponseAsync(string prompt, List<KnowledgeDocument> searchResults);
}