using System;
using System.Net.Http;
using Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blazor;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Registrer ApiEndpointResolver
        builder.Services.AddScoped<ApiEndpointResolver>();

        // Registrer HttpClient til API service
        builder.Services.AddHttpClient<APIService>(async (serviceProvider, client) =>
        {
            var resolver = serviceProvider.GetRequiredService<ApiEndpointResolver>();
            var endpoint = await resolver.ResolveApiEndpointAsync();
            client.BaseAddress = new Uri(endpoint);
            Console.WriteLine($"APIService BaseAddress: {client.BaseAddress}");
        });

        // Registrer generel HttpClient
        builder.Services.AddScoped(sp =>
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            };
            Console.WriteLine($"HttpClient BaseAddress: {client.BaseAddress}");
            return client;
        });

        await builder.Build().RunAsync();
    }
}
