using AzureRAGSystem.Domain.DomainModels;
using Microsoft.Azure.Cosmos;
using AzureRAGSystem.Repository.Interface;
using Container = Microsoft.Azure.Cosmos.Container;
using Microsoft.Extensions.Configuration;

namespace AzureRAGSystem.Repository.Implementation;

public class CosmosdbRepository : ICosmosdbRepository
{
    private readonly Container _container; // CRUD operations will be performed on this container
    
    public CosmosdbRepository(IConfiguration config)
    {
        var endpoint = config["CosmosDb:Endpoint"];
        var key = config["CosmosDb:Key"];
        var options = new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        };
        var databaseName = config["CosmosDb:DatabaseName"];
        var containerName = config["CosmosDb:ContainerName"];

        var client = new CosmosClient(endpoint, key, options);
        _container = client.GetContainer(databaseName, containerName);
    }
    
    public async Task<ChatSession> GetSessionByIdAsync(string sessionId, string userId)
    /*
     * This method asynchronously retrieves a ChatSession from the Cosmos DB container by its session ID
     * Requires userId for partition key optimization
     */
    {
        if (string.IsNullOrWhiteSpace(sessionId)) 
        {
            return null; 
        }
        
        try
        {
            ItemResponse<ChatSession> response = await _container.ReadItemAsync<ChatSession>(
                sessionId, 
                new PartitionKey(userId)
            );            
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)        
        {
            return null;
        }
    }

    public async Task<List<ChatSession>> GetUserSessionsAsync(string userId)
    /*
      This method asynchronously retrieves all ChatSession objects for a given user from the Cosmos DB container
      Uses userId as partition key for optimal performance
     */
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.userId = @userId")
            .WithParameter("@userId", userId);

        var iterator = _container.GetItemQueryIterator<ChatSession>(query, requestOptions: new QueryRequestOptions
        {
            PartitionKey = new PartitionKey(userId)
        });
        
        var sessions = new List<ChatSession>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            sessions.AddRange(response);
        }

        return sessions;
    }
    
    public async Task CreateSessionAsync(ChatSession session)
    /*
     * This method asynchronously creates a new ChatSession item in the Azure Cosmos DB container.
     * It uses the userId as the partition key to ensure proper data distribution and storage.
     */
    {
        if (session.Id == Guid.Empty)
        {
            session.Id = Guid.NewGuid();
        }
            
        await _container.CreateItemAsync(session, new PartitionKey(session.UserId));
    }

    public async Task SaveSessionAsync(ChatSession session)
    /*
      This method asynchronously saves a ChatSession object to the Azure Cosmos DB container
      Uses upsert to create or update the session with userId as partition key
     */
    {
        await _container.UpsertItemAsync(session, new PartitionKey(session.UserId));
    }
}