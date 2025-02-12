# Use the ASP.NET runtime image for the base stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
# Set a non-root user for better security
USER $APP_UID
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

# Switch to root user temporarily to apply chmod
USER root

# Set permissions on the /app directory to allow writing
RUN chmod -R 777 /app/appsettings.json \
    /app/NLog.config \
    /app/appsettings.qa.json \
    /app/NLog.qa.config \
    /app/appsettings.stage.json \
    /app/NLog.stage.config \
    /app/appsettings.prod.json \
    /app/NLog.prod.config

# Switch back to the non-root user
USER $APP_UID

# List the files in /app directory
RUN ls -al /app

# Set entry point to run the API
ENTRYPOINT ["dotnet", "DockerLearning.Api.dll"]
