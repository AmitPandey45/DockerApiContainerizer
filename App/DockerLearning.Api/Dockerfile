# Use the ASP.NET runtime image for the base stage
#FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
# Install necessary tools like jq for JSON manipulation
RUN apt-get update && apt-get install -y jq
# Install necessary tools like jq for JSON manipulation (using apk instead of apt-get)
#RUN apk update && apk add jq

# Build stage using the .NET SDK
#FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project files
COPY . .
RUN dotnet restore "App/DockerLearning.Api/DockerLearning.Api.csproj"

# Build the project
WORKDIR "/src/App/DockerLearning.Api"
RUN dotnet build "DockerLearning.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage: creating a self-contained, optimized package
FROM build AS publish
RUN dotnet publish "DockerLearning.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage: Production container
FROM base AS final
WORKDIR /inetpub/wwwroot

# Copy the published output from the publish stage
COPY --from=publish /app/publish .

# List the files in /app directory
RUN ls -al /inetpub/wwwroot

# Switch to root user temporarily to apply chmod
## USER root

# Set permissions on the /app directory to allow writing
## RUN chmod -R 777 /app/appsettings.json \
    #/app/NLog.config \
    #/app/appsettings.qa.json \
    #/app/NLog.qa.config \
    #/app/appsettings.stage.json \
    #/app/NLog.stage.config \
    #/app/appsettings.prod.json \
    #/app/NLog.prod.config

RUN chown -R $APP_UID:$APP_UID /inetpub/wwwroot

# Switch back to the non-root user
USER $APP_UID

# Ensure the update-config.sh script is executable
RUN chmod +x /inetpub/wwwroot/update-config.sh

# Remove Windows line endings from the script (if applicable)
RUN sed -i 's/\r//' /inetpub/wwwroot/update-config.sh

# Run the update-config.sh script to copy environment-specific configurations
RUN /bin/sh /inetpub/wwwroot/update-config.sh

# Set entry point to run the API
ENTRYPOINT ["dotnet", "DockerLearning.Api.dll"]
