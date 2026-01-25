using AzureRAGSystem.Repository.Interface;
using AzureRAGSystem.Infrastructure.Interface;
using AzureRAGSystem.Infrastructure.Implementation;
using AzureRAGSystem.Repository.Implementation;
using AzureRAGSystem.Service.Implementation;
using AzureRAGSystem.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICE REGISTRATION 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Blob Storage Service
var storageConnectionString = builder.Configuration.GetConnectionString("AzureBlobStorage");
builder.Services.AddScoped<IBlobService, BlobService>(sp => 
    new BlobService(storageConnectionString));

// Register Search Indexer Service
builder.Services.AddScoped<ISearchIndexerService, SearchIndexerService>(sp => 
{
    var endpoint = builder.Configuration["AzureSearch:Endpoint"];
    var apiKey = builder.Configuration["AzureSearch:ApiKey"];
    var indexName = builder.Configuration["AzureSearch:IndexName"];
    return new SearchIndexerService(endpoint, apiKey, indexName);
});

// CORS Policy: Allow both HTTP and HTTPS origins 
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins(
                "http://localhost:5001",   // HTTP UI
                "https://localhost:7100"   // HTTPS UI
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(origin => true) // Allow any origin in development
            .AllowCredentials();
    });
});

// Dependency Injection
builder.Services.AddScoped<ICosmosdbRepository, CosmosdbRepository>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IAzureOpenAIService, AzureOpenAIService>();
builder.Services.AddScoped<IRetrievalService, RetrievalService>();

// Register ChatService as a Typed Client
builder.Services.AddHttpClient<IChatService, ChatService>(client => {
    client.BaseAddress = new Uri("https://localhost:7242/"); 
});

// --- 2. BUILD THE APP ---
var app = builder.Build();

// --- 3. MIDDLEWARE PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(); 

app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Ok(new { 
    Status = "Online", 
    Project = "AzureRAGSystem API", 
    Documentation = "/swagger" 
}));app.Run();