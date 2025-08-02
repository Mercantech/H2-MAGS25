# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Build arguments for API endpoint
ARG API_ENDPOINT=http://localhost:8051/

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
WORKDIR "/src/Blazor"
RUN dotnet publish "Blazor.csproj" -c Release -o /app/publish --no-restore

# Runtime stage - use nginx with proper MIME type configuration
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Install curl for health check
RUN apk add --no-cache curl

# Copy published files
COPY --from=publish /app/publish/wwwroot .

# Set API endpoint as environment variable for runtime
ENV API_ENDPOINT=$API_ENDPOINT

# Copy nginx configuration
COPY Blazor/nginx.conf /etc/nginx/nginx.conf

# Create a simple health check script
RUN echo '#!/bin/sh' > /healthcheck.sh && \
    echo 'curl -f http://localhost/ || exit 1' >> /healthcheck.sh && \
    chmod +x /healthcheck.sh

# Verify that key files exist
RUN ls -la /usr/share/nginx/html/ && \
    ls -la /usr/share/nginx/html/_framework/ || echo "No _framework directory found"

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD /healthcheck.sh 