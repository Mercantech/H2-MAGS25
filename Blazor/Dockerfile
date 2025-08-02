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

# Runtime stage
FROM nginx:alpine AS final
ARG BUILD_DATE=unknown
ARG VCS_REF=unknown
LABEL org.label-schema.build-date=$BUILD_DATE \
      org.label-schema.vcs-ref=$VCS_REF

WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY Blazor/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80 