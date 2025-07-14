using System;
using System.Net.Http;
using Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Blazor;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp =>
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            };
            Console.WriteLine($"HttpClient BaseAddress: {client.BaseAddress}");
            return client;
        });

        builder.Services.AddHttpClient<APIService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7013/");
        });

        // Tilføj AuthState som singleton, så hele appen deler samme instans
        builder.Services.AddSingleton<AuthState>();

        var app = builder.Build();
        
        // Indlæs AuthState ved app start
        var authState = app.Services.GetRequiredService<AuthState>();
        await authState.LoadAsync(app.Services.GetRequiredService<IJSRuntime>());

        await app.RunAsync();
    }
}
