using AzureRAGSystem.Infrastructure.Implementation;
using AzureRAGSystem.Infrastructure.Interface;
using AzureRAGSystem.Service.Implementation;
using AzureRAGSystem.Service.Interface;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AzureRAGSystem.UI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// The root component usually maps to a <div> with id="app" in index.html
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Get the base address from the host environment (this will be the URL of the Web API serving the app)
var baseAddress = builder.HostEnvironment.BaseAddress;

builder.Services.AddHttpClient<IDocumentService, DocumentService>(client => 
{
    client.BaseAddress = new Uri(baseAddress); 
});

builder.Services.AddScoped<ISearchIndexerService, SearchIndexerService>();

// This allows your components to @inject IChatService
builder.Services.AddHttpClient<IChatService, ChatService>(client => 
{
    client.BaseAddress = new Uri(baseAddress); 
});

// Register the default HttpClient for components that inject HttpClient directly (like Index.razor)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });

// Build and run the WebAssembly app
await builder.Build().RunAsync();
