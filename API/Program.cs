using Scalar.AspNetCore;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }
            );
        });
        var app = builder.Build();

        // Use CORS
        app.UseCors("AllowAllOrigins");

        app.MapDefaultEndpoints();

        app.MapOpenApi();

        // Scalar Middleware for OpenAPI
        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("MAGSLearn")
                .WithTheme(ScalarTheme.Mars)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        // Map the Swagger UI
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "API v1");
        });

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
