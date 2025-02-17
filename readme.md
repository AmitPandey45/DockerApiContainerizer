Certainly! Below are the corrected commands, properly ordered and formatted for running your app in a local Docker container.

### Corrected and Ordered Commands

1. **Build the Docker image**:
   First, build the Docker image from your Dockerfile (this is the initial step).

   ```bash
   docker build --no-cache --progress plain -t amittest:1.0.1 -f ./App/Learning.Docker.Api/Dockerfile .
   ```

   - `--no-cache`: Ensures the build is done without using any cached layers.
   - `--progress plain`: Displays detailed build progress.
   - `-t amittest:1.0.1`: Tags the image as `amittest:1.0.1`.

2. **Run the Docker container with environment variable**:
   Now, run the Docker container with the specified environment variable (`ASPNETCORE_URLS`).

   ```bash
   docker run -d -p 7223:8080 -e ASPNETCORE_URLS=http://+:8080 amittest:1.0.1
   ```

   - `-d`: Runs the container in detached mode (in the background).
   - `-p 7223:8080`: Maps port `8080` inside the container to `7223` on your local machine.
   - `-e ASPNETCORE_URLS=http://+:8080`: Sets the environment variable to make the app listen on all network interfaces (`+`).

3. **Run the Docker container with a custom name and port mapping**:
   If you want to assign a name to the container and expose it on port `8081` locally, use:

   ```bash
   docker run --name dockerapi -d -p 8081:8080 amittest:1.0.1
   ```

   - `--name dockerapi`: Assigns the container the name `dockerapi`.
   - `-d`: Runs the container in detached mode.
   - `-p 8081:8080`: Maps port `8080` inside the container to port `8081` on your local machine.

### Summary of Commands in Correct Order:
```bash
# Step 1: Build the Docker image
docker build --no-cache --progress plain -t amittest:1.0.1 -f ./App/Learning.Docker.Api/Dockerfile .

# Step 2: Run the container with custom port and environment variable
docker run -d -p 7223:8080 -e ASPNETCORE_URLS=http://+:8080 amittest:1.0.1

# Step 3: Run the container with a name and custom port mapping
docker run --name dockerapi -d -p 8081:8080 amittest:1.0.1
```

Make sure you replace the image tags and ports as per your specific requirements.


docker build --no-cache --progress plain -t dockerlearning:1.0.0 -f ./App/DockerLearning.Api/Dockerfile .

docker run --name DockerLearningApi -d -p 7223:5432 dockerlearning:1.0.0


Set Environment Variables in Docker (if applicable):

If you're running your application in a Docker container, you can pass environment variables through the Docker -e flag:

docker run -e ASPNETCORE_ENVIRONMENT=Production -e MY_CUSTOM_ENV_VARIABLE=MyValue myapp:latest

docker build --no-cache --progress plain --build-arg ENVIRONMENT_NAME=TEST -t dockerlearning:1.0.0 -f ./App/DockerLearning.Api/Dockerfile .


docker run --name DockerLearningApi1 -d -p 8081:8080 -e ENVIRONMENT_NAME=DEV -e ASPNETCORE_ENVIRONMENT=dev -e MY_CUSTOM_ENV1=KeyEnv1ValueDev -e MY_CUSTOM_ENV2=KeyEnv2ValueDev dockerlearning:1.0.0

docker run --name DockerLearningApi2 -d -p 8082:8080 -e ENVIRONMENT_NAME=TEST -e ASPNETCORE_ENVIRONMENT=qa -e MY_CUSTOM_ENV1=KeyEnv1ValueQA -e MY_CUSTOM_ENV2=KeyEnv2ValueQA dockerlearning:1.0.0

docker run --name DockerLearningApi3 -d -p 8083:8080 -e ENVIRONMENT_NAME=STAGE -e ASPNETCORE_ENVIRONMENT=stage -e MY_CUSTOM_ENV1=KeyEnv1ValueStage -e MY_CUSTOM_ENV2=KeyEnv2ValueStage dockerlearning:1.0.0

docker run --name DockerLearningApi4 -d -p 8084:8080 -e ENVIRONMENT_NAME=PROD -e ASPNETCORE_ENVIRONMENT=prod -e MY_CUSTOM_ENV1=KeyEnv1ValueProd -e MY_CUSTOM_ENV2=KeyEnv2ValueProd dockerlearning:1.0.0



Passing the ARG Values During the Build
When building the Docker image using docker build, you pass the ARG values using the --build-arg flag. Here's how you'd do that:

docker build --build-arg build_env_name=dev --build-arg sonar_branch=feature-branch --build-arg sonar_token=your-sonar-token -t your-image-name .



#################################### MemS ############################


docker build --no-cache --progress plain -t dockerlearningservicelxpoc:1.0.0 -f ./App/dockerlearning.Api/Dockerfile .

docker run --name dockerlearningServiceLinuxApi -d -p 8085:8080 dockerlearningservicelxpoc:1.0.0

docker build -t dockerlearningservicelxpoc:1.0.0 -f ./App/dockerlearning.Api/Dockerfile .
docker run --name dockerlearningServiceLinuxApi -d -p 8085:8080 dockerlearningservicelxpoc:1.0.0


docker buildx build --no-cache --progress plain --platform linux/amd64 -t dockerlearningservicelxpoc:1.0.0 -f ./App/dockerlearning.Api/Dockerfile .
docker buildx build --no-cache --progress plain --platform linux/arm64 -t dockerlearningservicelxpoc:1.0.0 -f ./App/dockerlearning.Api/Dockerfile .
docker buildx build --no-cache --progress plain --platform linux/arm64,linux/amd64 -t dockerlearningservicelxpoc:1.0.0 -f ./App/dockerlearning.Api/Dockerfile .


docker build --no-cache --progress plain -t weatherapi:1.0.0 -f ./App/Weather.Api/Dockerfile .

docker run --name WeatherApi -d -p 8081:8080 weatherapi:1.0.0


docker build --build-arg sonar_token="" --build-arg sonar_branch="" --build-arg sonar_qualitygate=false --build-arg build_env_name=dev -t dockerlearningservicelxpoc:1.0.0 -f ./App/dockerlearning.Api/Dockerfile .

docker build --no-cache --progress plain --build-arg sonar_token="" --build-arg sonar_branch="" --build-arg sonar_qualitygate=false --build-arg build_env_name=dev -t dockerlearningservicelxpoc:1.0.0 -f ./App/dockerlearning.Api/Dockerfile .

==============================================CleanDockerRelatedSpaces ====================

docker system prune -a -f
docker builder prune -f

docker system df


=================================================== ENV VALEUS ===========================================

setx APPSETTING_AWSACCESSKEY "APPSETTING_AWSACCESSKEY_VALUE1"
setx APPSETTING_AWSSECRETACCESSKEY "APPSETTING_AWSSECRETACCESSKEY_VALUE1"
setx CONNSTR_DefaultDBConnStr "CONNSTR_DefaultDBConnStr_VALUE1"
setx ENVIRONMENT_NAME "DEV"
setx NLOGTARGET_TargetLocation "NLOGTARGET_TargetLocation_VALUE1"