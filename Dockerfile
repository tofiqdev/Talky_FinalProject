# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY BackNtier/ ./

# Restore dependencies
WORKDIR /src/Talky_API
RUN dotnet restore

# Build and publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Start application
ENTRYPOINT ["dotnet", "Talky_API.dll"]
