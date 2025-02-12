Great question! Here's a breakdown of when and how you pass the `ARG` variables like `build_env_name`, `sonar_branch`, and `sonar_token` in a Dockerfile setup:

### 1. **Defining Build Arguments in the Dockerfile (`ARG`)**

In your Dockerfile, you're defining `ARG` variables like this:

```dockerfile
ARG build_env_name=''
ARG sonar_branch=''
ARG sonar_token=''
```

These `ARG` variables are **build-time variables** that allow you to pass values when building the Docker image. However, **they are not available at runtime**—they are only accessible during the build process. You'll use them to control how the image is built, such as for environment-specific configurations or tool installations.

### 2. **Passing the `ARG` Values During the Build**

When building the Docker image using `docker build`, you pass the `ARG` values using the `--build-arg` flag. Here's how you'd do that:

```bash
docker build --build-arg build_env_name=dev --build-arg sonar_branch=feature-branch --build-arg sonar_token=your-sonar-token -t your-image-name .
```

- `--build-arg build_env_name=dev`: This sets the `build_env_name` argument to `"dev"`.
- `--build-arg sonar_branch=feature-branch`: This sets the `sonar_branch` argument to `"feature-branch"`.
- `--build-arg sonar_token=your-sonar-token`: This sets the `sonar_token` argument to `"your-sonar-token"`.

These `ARG` values will be available to the Dockerfile during the build process and can be used for conditional logic, like whether to run SonarQube analysis or install specific tools.

### 3. **Accessing the `ARG` Values in Dockerfile**

Once the `ARG` values are passed, they can be used anywhere in the Dockerfile. For example, in your case, you can use them in the `RUN` command like so:

```dockerfile
RUN if [ "$build_env_name" == "dev" ] || [ "$build_env_name" == "qa" ]; then \
    dotnet sonarscanner begin \
        /k:"mytest_MY-myservice" \
        /d:sonar.host.url="https://sonarcloud.io" \
        /o:"mytest" \
        /d:sonar.token="$sonar_token" \
        /d:sonar.branch.name="$sonar_branch" \
        /d:sonar.qualitygate.wait=true; \
    dotnet build "DockerLearning.Api.csproj" -c $BUILD_CONFIGURATION; \
    dotnet sonarscanner end /d:sonar.token="$sonar_token"; \
fi
```

This way, you can customize the build or run SonarQube analysis based on the passed `build_env_name`, `sonar_branch`, and `sonar_token`.

### 4. **Using Environment Variables (`ENV`) for Runtime Configuration**

If you need to pass these values to the container at **runtime** (when the container is running, not building), you'd use `ENV` to set environment variables instead of `ARG`.

For example:

```dockerfile
ENV SONAR_TOKEN=$sonar_token
ENV BUILD_ENV_NAME=$build_env_name
```

This allows your application to access these variables at runtime (using something like `System.Environment.GetEnvironmentVariable("BUILD_ENV_NAME")` in C#).

### 5. **Full Example of Usage**

Here’s how everything would work together:

```dockerfile
# Build stage using the .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG build_env_name=''
ARG sonar_branch=''
ARG sonar_token=''

WORKDIR /src

# Copy only the necessary files (CSProj) and restore dependencies
COPY ["App/DockerLearning.Api/DockerLearning.Api.csproj", "App/DockerLearning.Api/"]
RUN dotnet restore "App/DockerLearning.Api/DockerLearning.Api.csproj"

# Build the project
COPY . .
WORKDIR "/src/App/DockerLearning.Api"
RUN dotnet build "DockerLearning.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Sonar Analysis (only for dev or qa)
RUN if [ "$build_env_name" == "dev" ] || [ "$build_env_name" == "qa" ]; then \
    dotnet sonarscanner begin \
        /k:"mytest_MY-myservice" \
        /d:sonar.host.url="https://sonarcloud.io" \
        /o:"mytest" \
        /d:sonar.token="$sonar_token" \
        /d:sonar.branch.name="$sonar_branch" \
        /d:sonar.qualitygate.wait=true; \
    dotnet build "DockerLearning.Api.csproj" -c $BUILD_CONFIGURATION; \
    dotnet sonarscanner end /d:sonar.token="$sonar_token"; \
fi

# Publish Stage
FROM build AS publish
RUN dotnet publish "DockerLearning.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Copy the published output
COPY --from=publish /app/publish .

# Set entry point to run the API
ENTRYPOINT ["dotnet", "DockerLearning.Api.dll"]
```

### Summary of Steps:
1. **Pass Arguments During Build**: Use `--build-arg` to pass values like `build_env_name`, `sonar_branch`, and `sonar_token` to Docker at build time.
2. **Conditional Logic Based on Arguments**: You can use these arguments inside the Dockerfile to conditionally run commands like SonarQube analysis.
3. **Set Environment Variables for Runtime**: If you need the values at runtime, use `ENV` to set environment variables in the container.

Let me know if you need further clarification!