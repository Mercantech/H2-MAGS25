using System;
using System.Net.Http;
using Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

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
            client.BaseAddress = new Uri("http://localhost:5253/");
        });

        // Tilføj AuthState som singleton, så hele appen deler samme instans
        builder.Services.AddSingleton<AuthState>();

        await builder.Build().RunAsync();
    }
}
