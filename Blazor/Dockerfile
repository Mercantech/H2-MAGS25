# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["Blazor/Blazor.csproj", "Blazor/"]
COPY ["DomainModels/DomainModels.csproj", "DomainModels/"]
COPY ["API/API.csproj", "API/"]
COPY ["H2-Projekt.ServiceDefaults/H2-Projekt.ServiceDefaults.csproj", "H2-Projekt.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "Blazor/Blazor.csproj"

# Copy source code
COPY . .

# Build Blazor app
WORKDIR "/src/Blazor"
RUN dotnet build "Blazor.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Blazor.csproj" -c Release -o /app/publish

# Runtime stage - use ASP.NET Core runtime instead of nginx
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 80

ENTRYPOINT ["dotnet", "Blazor.dll"] 