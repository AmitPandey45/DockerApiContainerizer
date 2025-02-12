# Use the ASP.NET runtime image for the base stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
# Set a non-root user for better security
USER nobody:nogroup
WORKDIR /app
EXPOSE 8080 8081
EXPOSE 5000 7223

# Build stage using the .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only the necessary files (CSProj) and restore dependencies
COPY ["App/DockerLearning.Api/DockerLearning.Api.csproj", "App/DockerLearning.Api/"]
RUN dotnet restore "App/DockerLearning.Api/DockerLearning.Api.csproj"

# Copy the rest of the project files
COPY . .

# Build the project
WORKDIR "/src/App/DockerLearning.Api"
RUN dotnet build "DockerLearning.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage: creating a self-contained, optimized package
FROM build AS publish
RUN dotnet publish "DockerLearning.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage: Production container
FROM base AS final
WORKDIR /app

# Copy the published output from the publish stage
COPY --from=publish /app/publish .

# Set entry point to run the API
ENTRYPOINT ["dotnet", "DockerLearning.Api.dll"]
