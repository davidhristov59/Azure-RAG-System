using Azure;
using Azure.AI.OpenAI;
using AzureRAGSystem.Domain.DomainModels;
using AzureRAGSystem.Infrastructure.Interface;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace AzureRAGSystem.Infrastructure.Implementation;

public class AzureOpenAIService : IAzureOpenAIService
{
    private readonly AzureOpenAIClient _openAIClient;
    private readonly string _deploymentName;
    
    public AzureOpenAIService(IConfiguration configuration)
    {
        _openAIClient = new AzureOpenAIClient(
            new Uri(configuration["AzureOpenAI:Endpoint"]),
            new AzureKeyCredential(configuration["AzureOpenAI:ApiKey"]));
        
        _deploymentName = configuration["AzureOpenAI:DeploymentName"];
    }

    public async Task<string> GenerateResponseAsync(string prompt, List<KnowledgeDocument> searchResults)
        /*
          * AUGMENTATION & GENERATION
           This method combines the user's question with relevant knowledge base content to generate (GENERATION)
           contextually accurate responses. It configures the AI model with specific parameters
           and uses a system message to establish context and role, followed by the user's message.
        */
    {
        var context = string.Join("\n", searchResults.Select(doc => doc.Content)); // extracts content from all retrieved documents and joins them with newlines

        // message structure for chat completion
        var messages = new List<OpenAI.Chat.ChatMessage>()
        {
            // AUGMENTATION - combining retrieved context with user prompt
            new SystemChatMessage("You are a concise assistant. Answer the user's question " +
                                  "in 3-4 bullet points using ONLY the provided context. " +
                                  $"Context: {context}"),
            new UserChatMessage(prompt)
        };

        // send augmented prompt to the AI model
        var chatClient = _openAIClient.GetChatClient(_deploymentName);
        var response = await chatClient.CompleteChatAsync(messages);

        return response.Value.Content[0].Text;
    }
}