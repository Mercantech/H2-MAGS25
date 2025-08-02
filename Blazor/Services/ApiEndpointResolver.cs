using System.Net.Http;
using System.Text.Json;

namespace Blazor.Services;

public class ApiEndpointResolver
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiEndpointResolver> _logger;

    public ApiEndpointResolver(HttpClient httpClient, IConfiguration configuration, ILogger<ApiEndpointResolver> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> ResolveApiEndpointAsync()
    {
        // Først prøv miljøvariabler (Docker Compose)
        var dockerEndpoint = Environment.GetEnvironmentVariable("API_ENDPOINT_DOCKER");
        var localEndpoint = Environment.GetEnvironmentVariable("API_ENDPOINT_LOCAL");
        var productionEndpoint = Environment.GetEnvironmentVariable("API_ENDPOINT_PRODUCTION");

        // Derefter prøv appsettings.json
        var fallbackOrder = _configuration.GetSection("ApiEndpoints:FallbackOrder").Get<string[]>() 
            ?? new[] { "LocalDevelopment", "DockerCompose", "Production" };

        // Kombiner miljøvariabler og appsettings
        var endpoints = new List<(string name, string url)>
        {
            ("Docker", dockerEndpoint),
            ("Local", localEndpoint),
            ("Production", productionEndpoint)
        };

        // Tilføj appsettings endpoints hvis miljøvariabler ikke er sat
        if (string.IsNullOrEmpty(dockerEndpoint))
            endpoints.Add(("DockerCompose", _configuration["ApiEndpoints:DockerCompose"]));
        if (string.IsNullOrEmpty(localEndpoint))
            endpoints.Add(("LocalDevelopment", _configuration["ApiEndpoints:LocalDevelopment"]));
        if (string.IsNullOrEmpty(productionEndpoint))
            endpoints.Add(("Production", _configuration["ApiEndpoints:Production"]));

        foreach (var (name, endpoint) in endpoints)
        {
            if (string.IsNullOrEmpty(endpoint))
                continue;

            try
            {
                _logger.LogInformation("Testing API endpoint ({Name}): {Endpoint}", name, endpoint);
                
                // Test endpoint med en hurtig health check
                var response = await _httpClient.GetAsync($"{endpoint.TrimEnd('/')}/health");
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully connected to API endpoint ({Name}): {Endpoint}", name, endpoint);
                    return endpoint;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to connect to {Endpoint} ({Name}): {Error}", endpoint, name, ex.Message);
            }
        }

        // Hvis ingen endpoints virker, returner den første som fallback
        var firstEndpoint = endpoints.FirstOrDefault(e => !string.IsNullOrEmpty(e.url)).url;
        _logger.LogWarning("No API endpoints responded, using fallback: {Endpoint}", firstEndpoint);
        return firstEndpoint ?? "http://localhost:5253/";
    }
} 