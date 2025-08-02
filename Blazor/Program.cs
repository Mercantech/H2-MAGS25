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

        // Registrer HttpClient til API service med fast localhost:8051
        builder.Services.AddHttpClient<APIService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:8051/");
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
