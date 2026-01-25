using Azure;
using Azure.AI.OpenAI;
using AzureRAGSystem.Domain.DomainModels;
using AzureRAGSystem.Infrastructure.Interface;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.Text;

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
        // Build structured context with clear delimiters to help the model distinguish sources
        var contextBuilder = new StringBuilder();
        foreach (var (doc, index) in searchResults.Select((d, i) => (d, i)))
        {
            contextBuilder.AppendLine($"[Document {index + 1}]");
            contextBuilder.AppendLine(doc.Content);
            contextBuilder.AppendLine("---");
        }

        // message structure for chat completion
        var messages = new List<OpenAI.Chat.ChatMessage>()
        {
            // AUGMENTATION - combining retrieved context with user prompt
            new SystemChatMessage("You are a helpful and precise AI assistant. Use the following pieces of retrieved context to answer the user's question.\n" +
                                  "If the answer is not in the context, simply state that you don't have enough information.\n" +
                                  "Keep your response concise, professional, and well-formatted.\n\n" +
                                  $"Context Data:\n{contextBuilder}"),
            new UserChatMessage(prompt)
        };

        // send augmented prompt to the AI model
        var chatClient = _openAIClient.GetChatClient(_deploymentName);
        var response = await chatClient.CompleteChatAsync(messages);

        return response.Value.Content[0].Text;
    }
}