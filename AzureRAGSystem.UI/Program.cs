using AzureRAGSystem.Service.Implementation;
using AzureRAGSystem.Service.Interface;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AzureRAGSystem.UI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// The root component usually maps to a <div> with id="app" in index.html
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// This allows your components to @inject IChatService
builder.Services.AddHttpClient<IChatService, ChatService>(client => 
{
    // Points to our Backend API URL
    // The UI (port 5000) calls the API (port 7242)
    client.BaseAddress = new Uri("https://localhost:7242/"); 
});

// Build and run the WebAssembly app
await builder.Build().RunAsync();

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// await builder.Build().RunAsync();