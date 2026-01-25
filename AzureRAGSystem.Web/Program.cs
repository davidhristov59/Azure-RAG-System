using AzureRAGSystem.Repository.Interface;
using AzureRAGSystem.Infrastructure.Interface;
using AzureRAGSystem.Infrastructure.Implementation;
using AzureRAGSystem.Repository.Implementation;
using AzureRAGSystem.Service.Implementation;
using AzureRAGSystem.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add Console Logging for debugging on Azure
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// --- 1. SERVICE REGISTRATION 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Blob Storage Service 
var storageConnectionString = builder.Configuration.GetConnectionString("AzureBlobStorage") 
                           ?? builder.Configuration["AzureBlobStorage"];

if (string.IsNullOrEmpty(storageConnectionString)) 
{
    Console.WriteLine("CRITICAL: AzureBlobStorage connection string is missing in both ConnectionStrings and AppSettings!");
    throw new InvalidOperationException("AzureBlobStorage connection string is not configured.");
}

builder.Services.AddScoped<IBlobService, BlobService>(sp => 
    new BlobService(storageConnectionString));

// Register Search Indexer Service
builder.Services.AddScoped<ISearchIndexerService, SearchIndexerService>(sp => 
{
    var endpoint = builder.Configuration["AzureSearch:Endpoint"];
    var apiKey = builder.Configuration["AzureSearch:ApiKey"];
    var indexName = builder.Configuration["AzureSearch:IndexName"];
    
    if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(indexName))
    {
        Console.WriteLine($"CRITICAL: Azure Search Config Missing. Endpoint: {endpoint!=null}, ApiKey: {apiKey!=null}, IndexName: {indexName!=null}");
        throw new InvalidOperationException("Azure Search configuration is incomplete!");
    }
    
    return new SearchIndexerService(endpoint, apiKey, indexName);
});

// CORS Policy
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins(
                "http://localhost:5001",   
                "https://localhost:7100",  
                "https://azure-rag-platform-gudwa2bjhehme5hf.italynorth-01.azurewebsites.net" 
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Dependency Injection
builder.Services.AddScoped<ICosmosdbRepository, CosmosdbRepository>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IAzureOpenAIService, AzureOpenAIService>();
builder.Services.AddScoped<IRetrievalService, RetrievalService>();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];

// Register Services for HttpClient injection
builder.Services.AddHttpClient<IChatService, ChatService>(client => {
    client.BaseAddress = new Uri(apiBaseUrl ?? "https://localhost:7242/");
});
builder.Services.AddHttpClient<IDocumentService, DocumentService>(client => {
    client.BaseAddress = new Uri(apiBaseUrl ?? "https://localhost:7242/");
});
builder.Services.AddScoped(sp => new HttpClient {
    BaseAddress = new Uri(apiBaseUrl ?? "https://localhost:7242/")
});

// --- 2. BUILD THE APP ---
var app = builder.Build();

// --- 3. MIDDLEWARE PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging(); // Enable debugging for Blazor WASM
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// --- Blazor WASM Hosting ---
// Serves the Blazor WASM framework files (_framework/*)
app.UseBlazorFrameworkFiles();
// Serves other static files from wwwroot (CSS, JS, images)
app.UseStaticFiles();

app.UseRouting();
app.UseCors(); 
app.UseAuthorization();

// Map API controllers
app.MapControllers();

// Fallback to the Blazor app's index.html for any requests that aren't for static files or API endpoints.
app.MapFallbackToFile("index.html");

app.Run();