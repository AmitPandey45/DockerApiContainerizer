sonarqube

UP admin/Ke..7

Hey GM!

It looks like you want to migrate from a Windows-based Dockerfile to a Linux-based one for your .NET 8 API. You've got the basics of the Linux Dockerfile, but you're wondering how to integrate the same functionality you had in the Windows Dockerfile, such as handling build environments, dependencies, Sonar analysis, and monitoring tools.

Here's how we can add those extra functionalities to your Linux Dockerfile:

### 1. **Handling Build Arguments (Environment Variables)**

In your Windows Dockerfile, you used PowerShell's `$env:build_env_name` to manage build arguments. On Linux, you'd typically use `ARG` and `ENV` for this. So, for things like `build_env_name`, `sonar_branch`, and `sonar_token`, you'd define them as build arguments (`ARG`) and environment variables (`ENV`).

### 2. **Installing .NET SDK and Tools**

In your Windows Dockerfile, you manually downloaded the .NET SDK and other tools using PowerShell. On Linux, you can leverage the Alpine-based image for .NET and install additional tools as needed via `apk` (Alpine's package manager) or `dotnet tool` commands.

### 3. **SonarQube Analysis**

For SonarQube, you can use the same `dotnet sonarscanner` commands, but you'll need to make sure that any required tools (like `dotnet-sonarscanner`) are installed in the container.

### 4. **Service Monitoring (e.g., DataDog, Service Monitor)**

In your Windows Dockerfile, you used `Invoke-WebRequest` to download monitoring tools. On Linux, you can download these tools with `curl` or `wget`. The setup process for DataDog APM or any other monitoring tools would differ slightly on Linux.

Here’s a revised version of your Linux Dockerfile, incorporating these elements:

```dockerfile
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
ARG build_env_name=''
ARG sonar_branch=''
ARG sonar_token=''
WORKDIR /src

# Install dependencies: curl for downloading tools, and .NET tools for SonarQube analysis
RUN apk add --no-cache curl bash

# Install dotnet-sonarscanner globally (for SonarQube integration)
RUN dotnet tool install --global dotnet-sonarscanner

# Copy only the necessary files (CSProj) and restore dependencies
COPY ["App/DockerLearning.Api/DockerLearning.Api.csproj", "App/DockerLearning.Api/"]
RUN dotnet restore "App/DockerLearning.Api/DockerLearning.Api.csproj"

# Copy the rest of the project files
COPY . .

# Build the project
WORKDIR "/src/App/DockerLearning.Api"
RUN dotnet build "DockerLearning.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Sonar Analysis Stage (only if dev/qa)
RUN if [ "$build_env_name" == "dev" ] || [ "$build_env_name" == "qa" ]; then \
    dotnet sonarscanner begin \
        /k:"mytest_MY-myservice_MY-standardapi" \
        /d:sonar.host.url="https://sonarcloud.io" \
        /o:"mytest_MY-myservice" \
        /d:sonar.token="$sonar_token" \
        /d:sonar.branch.name="$sonar_branch" \
        /d:sonar.qualitygate.wait=true; \
    dotnet build "DockerLearning.Api.csproj" -c $BUILD_CONFIGURATION; \
    dotnet sonarscanner end /d:sonar.token="$sonar_token"; \
fi

# Publish stage: creating a self-contained, optimized package
FROM build AS publish
RUN dotnet publish "DockerLearning.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage: Production container
FROM base AS final
WORKDIR /app

# Copy the published output from the publish stage
COPY --from=publish /app/publish .

# Install DataDog APM (example)
RUN curl -L https://github.com/DataDog/dd-trace-dotnet/releases/download/v2.49.0/datadog-dotnet-apm-2.49.0-linux-x64.tar.gz | tar xz -C /app

# Set entry point to run the API
ENTRYPOINT ["dotnet", "DockerLearning.Api.dll"]
```

### Key Changes/Features:
1. **Environment Variables**: `ARG build_env_name`, `ARG sonar_branch`, and `ARG sonar_token` are passed as build arguments and used in the SonarQube analysis.
   
2. **SonarQube**: `dotnet sonarscanner` is installed globally, and we only run the SonarQube analysis if the `build_env_name` is `dev` or `qa`.
   
3. **Monitoring Tools**: I added an example of installing the DataDog APM tool using `curl` to download the tarball and extracting it.

4. **Security Considerations**: The base image uses a non-root user `nobody:nogroup` for better security.

### Considerations:
- You can install additional monitoring tools or services by modifying the `RUN` commands for downloading and setting them up.
- Depending on your exact needs, you may want to adjust the SonarQube integration or customize environment variables more.
- The APM or Service Monitoring setup might need further configuration depending on the monitoring service you're using.

Let me know if you need further adjustments or clarifications!







Got it! You want to set up **SonarQube** without PostgreSQL and just use SonarQube itself. SonarQube can run with an embedded H2 database for simpler setups, but it's highly recommended to use PostgreSQL or other databases in production. However, for local development or testing, you can use the embedded H2 database.

Here’s how you can set up **SonarQube with Docker** using only SonarQube (no PostgreSQL):

### Step 1: Create a `docker-compose.yml` for SonarQube with H2 Database

Since you're only interested in SonarQube without PostgreSQL, we’ll use the SonarQube image with its default **H2 database** for a local setup.

1. **Create the `docker-compose.yml` file**:

```yaml
version: '3'
services:
  sonarqube:
    image: sonarqube:latest
    container_name: sonarqube
    environment:
      - SONARQUBE_JDBC_URL=jdbc:h2:tcp://localhost:9092/sonar
      - SONARQUBE_JDBC_USERNAME=sonar
      - SONARQUBE_JDBC_PASSWORD=sonar
    ports:
      - "9000:9000"  # Exposes port 9000 to localhost
    volumes:
      - sonarqube_data:/opt/sonarqube/data
      - sonarqube_extensions:/opt/sonarqube/extensions
      - sonarqube_logs:/opt/sonarqube/logs
    networks:
      - sonarnetwork

networks:
  sonarnetwork:
    driver: bridge

volumes:
  sonarqube_data:
  sonarqube_extensions:
  sonarqube_logs:
```

This configuration will:
- Use **SonarQube** with the default **H2 embedded database**.
- Expose **SonarQube** on port `9000`.
- Persist SonarQube data, extensions, and logs to Docker volumes for data persistence.

### Step 2: Start SonarQube using Docker Compose

Now, let's run SonarQube locally with the embedded H2 database:

1. **Run the container using Docker Compose**:

```bash
docker-compose up -d
```

This will start the SonarQube container with the H2 database and expose it at `http://localhost:9000`.

### Step 3: Access SonarQube

- After running `docker-compose up -d`, SonarQube should be running.
- Open your browser and navigate to **http://localhost:9000**.
- The default login credentials are:
  - **Username**: `admin`
  - **Password**: `admin`

### Step 4: Expose SonarQube Publicly (Optional)

If you want SonarQube to be publicly accessible (for example, if you're running this on a server and want to access it from anywhere), follow these steps:

1. **Ensure your firewall allows traffic on port 9000** if you’re using a cloud server.
2. **Use a reverse proxy (optional)**: You can use **NGINX** or **ngrok** to make SonarQube accessible via a public URL. Here’s how you can expose it using **ngrok**:

#### Option 1: Exposing via `ngrok`
   
- Install `ngrok` if you haven’t already (refer to [ngrok installation guide](https://ngrok.com/download)).
- Run `ngrok` to expose your local SonarQube server:

```bash
ngrok http 9000
```

This will provide you with a **public URL** like `http://<your-ngrok-subdomain>.ngrok.io`, which you can use to access SonarQube publicly.

#### Option 2: Using NGINX (For a production-like setup)

If you want to use **NGINX** as a reverse proxy to expose SonarQube, follow the steps in the earlier message about configuring **NGINX**.

---

### Step 5: Running SonarQube Analysis on Your Local Projects

Once SonarQube is running, you can use **SonarScanner** to run analysis on your local projects.

#### Install SonarScanner

You can install **SonarScanner** to analyze your project:

1. **For Linux/macOS**, you can install it via Homebrew:

   ```bash
   brew install sonarqube
   ```

2. **For Windows**, download the latest SonarScanner from the [SonarQube download page](https://docs.sonarqube.org/latest/analysis/scan/sonarscanner/).

#### Configure the `sonar-scanner.properties` file

The `sonar-scanner.properties` file is used to configure the connection to your SonarQube instance. Make sure it points to `http://localhost:9000` for the server URL:

```properties
sonar.host.url=http://localhost:9000
sonar.login=your-sonar-token  # Optional: You can generate a token from SonarQube
```

To generate a **SonarQube token**:
1. Go to your SonarQube instance (e.g., `http://localhost:9000`).
2. Navigate to **My Account → Security** and generate a new token.

#### Running SonarQube Analysis

To analyze a project:

1. Navigate to the root of your project.
2. Run the following command:

```bash
sonar-scanner
```

This will analyze your project and send the results to your local SonarQube server.

---

### Summary:

1. **Set up SonarQube with Docker using the embedded H2 database**.
2. **Expose SonarQube via `localhost:9000`** for local access.
3. Optionally, expose SonarQube publicly using **ngrok** or a **reverse proxy** (NGINX).
4. **Run SonarQube analysis** on your local projects using **SonarScanner**.

This setup is ideal for local development and testing. Let me know if you need more assistance!



######################################################################################################


Yes, **you can run SonarQube on Windows without installing PostgreSQL** by using the **embedded H2 database**, which comes bundled with SonarQube. This is a simple option, especially for **evaluation** or **development purposes**, but it is **not recommended for production environments** as H2 has limitations in terms of scalability and performance.

### Running SonarQube with the Embedded H2 Database

Here's how you can set up SonarQube without PostgreSQL, using the embedded **H2** database:

### Step 1: Download and Install SonarQube

1. **Download SonarQube** from the [SonarQube Downloads Page](https://www.sonarqube.org/downloads/).

2. **Extract the ZIP file** to a folder on your local machine.

### Step 2: Configure SonarQube to Use the H2 Database

By default, SonarQube is configured to use the **embedded H2 database**. You don't need to change anything if you intend to use the H2 database, as it is already set up by default.

1. Navigate to the **SonarQube** installation directory.
2. Go to the **`conf`** folder and open the `sonar.properties` file in a text editor.
3. The default configuration uses H2, so you do not need to modify the database configuration lines unless you plan to use a different database (like PostgreSQL). The lines should look something like this:

   ```properties
   # Embedded H2 database (default)
   sonar.jdbc.url=jdbc:h2:tcp://localhost:9092/sonar;DB_CLOSE_ON_EXIT=FALSE;AUTO_SERVER=TRUE
   sonar.jdbc.username=sa
   sonar.jdbc.password=
   ```

   This configuration points SonarQube to use H2 running on your local machine, so you don't need PostgreSQL or any other external database.

### Step 3: Start SonarQube

1. Navigate to the **`bin/windows-x86-64`** directory inside the SonarQube folder.
2. Double-click on **`StartSonar.bat`** to start SonarQube.

   Alternatively, you can open a **Command Prompt** in that directory and run:

   ```bash
   StartSonar.bat
   ```

3. Wait for SonarQube to start up. It might take a couple of minutes. The console will indicate when the system is ready.

### Step 4: Access SonarQube

1. Open your web browser and navigate to **http://localhost:9000**.
2. The default login credentials are:
   - **Username**: `admin`
   - **Password**: `admin`

### Step 5: Run SonarQube Analysis

1. **Install SonarScanner**:
   - Download and install **SonarScanner** from [here](https://docs.sonarqube.org/latest/analysis/scan/sonarscanner/).
   
2. **Configure SonarScanner**:
   - Open the `conf/sonar-scanner.properties` file and set the `sonar.host.url` to:

   ```properties
   sonar.host.url=http://localhost:9000
   sonar.login=<your-sonar-token>  # Optional: Create this in the SonarQube UI
   ```

3. **Run the analysis**:
   - Open a **Command Prompt** in your project directory and run:

   ```bash
   sonar-scanner
   ```

4. After the scan completes, go to **http://localhost:9000** to see the results in the SonarQube dashboard.

### Summary

- **PostgreSQL is not required** if you're just testing or developing with SonarQube.
- You can use the embedded **H2 database** that comes with SonarQube for local development.
- The default SonarQube configuration is already set to use H2, so you don’t need to modify the `sonar.properties` file unless you want to configure a different database.

However, **keep in mind that H2 is not suitable for production**. If you're planning to deploy SonarQube for a real-world, production-level project, using a more robust database like **PostgreSQL** or **MySQL** is recommended.

Let me know if you need any further help!






##############################################################################################################


The `sonar-scanner.properties` file is part of the **SonarScanner** installation, not SonarQube itself. It's used to configure the **SonarScanner** tool, which sends your project analysis data to SonarQube.

### Where to Find `sonar-scanner.properties` File:

After you download and install **SonarScanner**, follow these steps to locate the `sonar-scanner.properties` file:

1. **Download SonarScanner**:
   - Go to the official [SonarScanner download page](https://docs.sonarqube.org/latest/analysis/scan/sonarscanner/) and download the SonarScanner for your operating system.

2. **Extract the ZIP file**:
   - Once downloaded, extract the SonarScanner ZIP file to a location of your choice on your system.

3. **Locate `sonar-scanner.properties`**:
   - Inside the extracted folder, navigate to the **`conf`** directory.
   - You'll find the `sonar-scanner.properties` file inside this `conf` folder.

For example, if you extracted SonarScanner to `C:\SonarScanner`, you should find the `sonar-scanner.properties` file at:

```
C:\SonarScanner\conf\sonar-scanner.properties
```

### Configure `sonar-scanner.properties`

Once you find the `sonar-scanner.properties` file, you need to edit it to point SonarScanner to your **SonarQube instance** (running on `localhost` if you're using it locally).

1. Open the `sonar-scanner.properties` file in a text editor (like Notepad).
2. Look for the following line:

   ```properties
   # Mandatory configuration
   sonar.host.url=http://localhost:9000
   ```

   This line tells SonarScanner where your SonarQube instance is running. Make sure this is set to the correct URL (`http://localhost:9000` for a local SonarQube instance).

3. **Optional**: If you're using a SonarQube **authentication token**, you can also add this line in the same file:

   ```properties
   sonar.login=<your-sonar-token>
   ```

   Replace `<your-sonar-token>` with the actual token. You can generate a token from the **SonarQube** UI under **My Account** → **Security** → **Generate Tokens**.

### Example Configuration:

Here’s a sample of what the `sonar-scanner.properties` file might look like after configuration:

```properties
# Configure the SonarQube server URL
sonar.host.url=http://localhost:9000

# Authentication token (optional but recommended)
sonar.login=your-sonar-token
```

After configuring this file, you can run **SonarScanner** to analyze your project and send the results to your local SonarQube instance.

### Running SonarScanner:

Once your `sonar-scanner.properties` is set up, you can run the scanner from your project directory:

1. Open **Command Prompt** or **PowerShell** in your project’s root folder.
2. Run the following command:

   ```bash
   sonar-scanner
   ```

3. The analysis will run, and after completion, you can view the results on your **SonarQube Dashboard** at **http://localhost:9000**.

Let me know if you need further help with this setup!




======================================================================================================================================

Run analysis on your project
What option best describes your project?
Maven
Gradle
.NET
Other (for JS, TS, Go, Python, PHP, ...)
Which framework do you use?
.NET Core
.NET Framework
Install the SonarScanner .NET Core Global Tool
As a prerequisite you need to have the sonarscanner tool installed globally using the following command:

dotnet tool install --global dotnet-sonarscanner

Execute the Scanner
Running a SonarQube analysis is straightforward. You just need to execute the following commands at the root of your solution.

dotnet sonarscanner begin /k:"dockerlearningservicelxpoc" /d:sonar.host.url="https://0208-2409-40e3-30ab-e698-54be-512a-2a69-2266.ngrok-free.app"  /d:sonar.token="sqp_b1da8c4c23c9c51b591497ad6db8002826ef66a0"
dotnet build
dotnet sonarscanner end /d:sonar.token="sqp_b1da8c4c23c9c51b591497ad6db8002826ef66a0"



dotnet sonarscanner begin /k:"dockerlearningservicelxpoc" /d:sonar.host.url="https://0208-2409-40e3-30ab-e698-54be-512a-2a69-2266.ngrok-free.app"  /d:sonar.token="sqp_b1da8c4c23c9c51b591497ad6db8002826ef66a0" /d:sonar.cs.vscoveragexml.reportsPaths="App/dockerlearning.Api/dockerlearningSystem_CoverageReport.xml" /d:sonar.exclusions="**/*.pem,**/*.sql,**/kube--deploy*.yaml,**/AssemblyInfo.cs,App/dockerlearning.Api/Properties/**,App/dockerlearning.Api/logs/**" /d:sonar.coverage.exclusions="**/ServiceMapperProfile.cs,**/kube-deploy*.yaml,**/*.config,**/Dockerfile,**/*.ps1,**/Tests/**,**/dockerlearning.Api/App_Start/**,**/Common/**,**/Core/**" /d:sonar.sources="App/dockerlearning.Api"


dotnet sonarscanner begin \
        /k:"dockerlearningservicelxpoc" \
        /d:sonar.host.url="https://0208-2409-40e3-30ab-e698-54be-512a-2a69-2266.ngrok-free.app" \
        /d:sonar.token="sqp_b1da8c4c23c9c51b591497ad6db8002826ef66a0" \
        /d:sonar.branch.name="main" \
        /d:sonar.qualitygate.wait=false \
        /d:sonar.cs.vscoveragexml.reportsPaths="App/dockerlearning.Api/dockerlearningSystem_CoverageReport.xml" \
        /d:sonar.exclusions="**/*.pem,**/*.sql,**/kube--deploy*.yaml,**/AssemblyInfo.cs,App/dockerlearning.Api/Properties/**,App/dockerlearning.Api/logs/**" \
        /d:sonar.coverage.exclusions="**/ServiceMapperProfile.cs,**/kube-deploy*.yaml,**/*.config,**/Dockerfile,**/*.ps1,**/Tests/**,**/dockerlearning.Api/App_Start/**,**/Common/**,**/Core/**"; \
    dotnet build "MY.dockerlearning.sln" --no-incremental; \
    dotnet-coverage collect 'dotnet test MY.dockerlearning.sln' -f xml -o 'App/dockerlearning.Api/dockerlearningSystem_CoverageReport.xml'; \
    dotnet sonarscanner end /d:sonar.token="$env:sonar_token" \
	

dotnet sonarscanner begin \
  /k:"dockerlearningservicelxpoc" \
  /d:sonar.host.url="https://0208-2409-40e3-30ab-e698-54be-512a-2a69-2266.ngrok-free.app" \
  /d:sonar.token="sqp_b1da8c4c23c9c51b591497ad6db8002826ef66a0" \
  /d:sonar.branch.name="main" \
  /d:sonar.qualitygate.wait=false \
  /d:sonar.cs.vscoveragexml.reportsPaths="App/dockerlearning.Api/dockerlearningSystem_CoverageReport.xml" \
  /d:sonar.exclusions="**/*.pem,**/*.sql,**/kube--deploy*.yaml,**/AssemblyInfo.cs,App/dockerlearning.Api/Properties/**,App/dockerlearning.Api/logs/**" \
  /d:sonar.coverage.exclusions="**/ServiceMapperProfile.cs,**/kube-deploy*.yaml,**/*.config,**/Dockerfile,**/*.ps1,**/Tests/**,**/dockerlearning.Api/App_Start/**,**/Common/**,**/Core/**" \
  /d:sonar.projectBaseDir="D:/Projects/MY-services" \
  /d:sonar.sln="MY.dockerlearning.sln"


dotnet sonarscanner begin \
  /k:"dockerlearningservicelxpoc" \
  /d:sonar.host.url="https://0208-2409-40e3-30ab-e698-54be-512a-2a69-2266.ngrok-free.app" \
  /d:sonar.token="sqp_b1da8c4c23c9c51b591497ad6db8002826ef66a0" \
  /d:sonar.branch.name="main" \
  /d:sonar.qualitygate.wait=false \
  /d:sonar.cs.vscoveragexml.reportsPaths="App/dockerlearning.Api/dockerlearningSystem_CoverageReport.xml" \
  /d:sonar.exclusions="**/*.pem,**/*.sql,**/kube--deploy*.yaml,**/AssemblyInfo.cs,App/dockerlearning.Api/Properties/**,App/dockerlearning.Api/logs/**" \
  /d:sonar.coverage.exclusions="**/ServiceMapperProfile.cs,**/kube-deploy*.yaml,**/*.config,**/Dockerfile,**/*.ps1,**/Tests/**,**/dockerlearning.Api/App_Start/**,**/Common/**,**/Core/**" \
  /d:sonar.projectFile="App/dockerlearning.Api/dockerlearning.Api.csproj"

	

dotnet sonarscanner begin k:"dockerlearningservicelxpoc" d:sonar.host.url="https://0208-2409-40e3-30ab-e698-54be-512a-2a69-2266.ngrok-free.app" d:sonar.token="sqp_b1da8c4c23c9c51b591497ad6db8002826ef66a0" d:sonar.branch.name="main" d:sonar.qualitygate.wait=false d:sonar.cs.vscoveragexml.reportsPaths="App/dockerlearning.Api/dockerlearningSystem_CoverageReport.xml" d:sonar.exclusions="**/*.pem,**/*.sql,**/kube--deploy*.yaml,**/AssemblyInfo.cs,App/dockerlearning.Api/Properties/**,App/dockerlearning.Api/logs/**" d:sonar.coverage.exclusions="**/ServiceMapperProfile.cs,**/kube-deploy*.yaml,**/*.config,**/Dockerfile,**/*.ps1,**/Tests/**,**/dockerlearning.Api/App_Start/**,**/Common/**,**/Core/**" d:sonar.projectBaseDir="D:/Projects/MY-services" d:sonar.projectFile="D:/Projects/MY-services/App/dockerlearning.Api/dockerlearning.Api.csproj"

    dotnet build "MY.dockerlearning.sln" --no-incremental; \
    dotnet-coverage collect 'dotnet test MY.dockerlearning.sln' -f xml -o 'App/dockerlearning.Api/dockerlearningSystem_CoverageReport.xml'; \
    dotnet sonarscanner end /d:sonar.token="$env:sonar_token" \
	
	
	
	
	
	
============================================================================================================


dotnet test "MY.dockerlearning.sln" --logger "trx;LogFileName=App/dockerlearning.Api/test-results.xml"


dotnet sonarscanner begin /k:"dockerlearningservicelxpoc" /d:sonar.host.url="https://0208-2409-40e3-30ab-e698-54be-512a-2a69-2266.ngrok-free.app" /o:"mytest_MY-myservice" /d:sonar.token="sqp_b1da8c4c23c9c51b591497ad6db8002826ef66a0" /d:sonar.branch.name="main" /d:sonar.qualitygate.wait=true /d:sonar.cs.vscoveragexml.reportsPaths="App/dockerlearning.Api/dockerlearningSystem_CoverageReport.xml" /d:sonar.exclusions="**/*.pem,**/*.sql,**/kube--deploy*.yaml,**/AssemblyInfo.cs,App/dockerlearning.Api/Properties/**,App/dockerlearning.Api/logs/**" /d:sonar.coverage.exclusions="**/ServiceMapperProfile.cs,**/kube-deploy*.yaml,**/*.config,**/Dockerfile,**/*.ps1,**/Tests/**,**/dockerlearning.Api/App_Start/**,**/Common/**,**/Core/**" /d:sonar.projectBaseDir="D:/Projects/MY-services" /d:sonar.sources="App/dockerlearning.Api" /d:sonar.projectFile="App/dockerlearning.Api/dockerlearning.Api.csproj"

dotnet build "dockerlearning.Api.csproj" --no-incremental
dotnet build "MY.dockerlearning.sln" --no-incremental

dotnet-coverage collect "dotnet test dockerlearning.Api.csproj" -f xml -o "App/dockerlearning.Api/dockerlearningSystem_CoverageReport.xml" -->Good ONE correct
dotnet-coverage collect "dotnet test MY.dockerlearning.sln" -f xml -o "App/dockerlearning.Api/dockerlearningSystem_CoverageReport.xml" -->Good ONE correct

dotnet sonarscanner end /d:sonar.token="sqp_b1da8c4c23c9c51b591497ad6db8002826ef66a0"


dotnet sonarscanner begin /k:"dockerlearningservicelxpoc" /d:sonar.host.url="http://localhost:9000/" /d:sonar.token="sqp_b1da8c4c23c9c51b591497ad6db8002826ef66a0" /d:sonar.cs.vscoveragexml.reportsPaths="dockerlearningsystem_coveragereport.xml" /d:sonar.exclusions="**/*.pem,**/*.sql,**/kube--deploy*.yaml,**/AssemblyInfo.cs,App/dockerlearning.Api/Properties/**,App/dockerlearning.Api/logs/**" /d:sonar.coverage.exclusions="**/ServiceMapperProfile.cs,**/kube-deploy*.yaml,**/*.config,**/Dockerfile,**/*.ps1,**/Tests/**,**/dockerlearning.Api/App_Start/**,**/Common/**,**/Core/**" /d:sonar.scanner.scanAll=false

dotnet build "dockerlearning.Api.csproj" --no-incremental

dotnet-coverage collect "dotnet test dockerlearning.Api.csproj" -f xml -o "dockerlearningsystem_coveragereport.xml"

dotnet sonarscanner end /d:sonar.token="sqp_b1da8c4c23c9c51b591497ad6db8002826ef66a0"


dotnet-coverage collect "dotnet test MY.dockerlearning.sln" -f xml -o "App/dockerlearning.Api/dockerlearningsystem_coveragereport.xml"


============================ Corret ==================================

dotnet sonarscanner begin /k:"dockerlearningservicelxpoc" /d:sonar.host.url="http://localhost:9000/" /d:sonar.token="sqp_b1da8c4c23c9c51b591497ad6db8002826ef66a0" /d:sonar.cs.vscoveragexml.reportsPaths="App/dockerlearning.Api/dockerlearningSystem_CoverageReport.xml" /d:sonar.exclusions="**/*.pem,**/*.sql,**/kube--deploy*.yaml,**/AssemblyInfo.cs,App/dockerlearning.Api/Properties/**,App/dockerlearning.Api/logs/**" /d:sonar.coverage.exclusions="**/ServiceMapperProfile.cs,**/kube-deploy*.yaml,**/*.config,**/Dockerfile,**/*.ps1,**/Tests/**,**/dockerlearning.Api/App_Start/**,**/Common/**,**/Core/**" /d:sonar.scanner.scanAll=false


dotnet sonarscanner end /d:sonar.token="sqp_b1da8c4c23c9c51b591497ad6db8002826ef66a0"




====================================================================

dotnet sonarscanner begin /k:"dockerlearningservicelxpoc" /d:sonar.host.url="http://localhost:9000/" /d:sonar.token="sqp_b1da8c4c23c9c51b591497ad6db8002826ef66a0" /d:sonar.exclusions="**/*.pem,**/*.sql,**/kube--deploy*.yaml,**/AssemblyInfo.cs,App/dockerlearning.Api/Properties/**,App/dockerlearning.Api/logs/**" /d:sonar.coverage.exclusions="**/ServiceMapperProfile.cs,**/kube-deploy*.yaml,**/*.config,**/Dockerfile,**/*.ps1,**/Tests/**,**/dockerlearning.Api/App_Start/**,**/Common/**,**/Core/**" /d:sonar.scanner.scanAll=false

dotnet test "MY.dockerlearning.sln" --logger "trx" --collect:"Code Coverage" --results-directory "App/dockerlearning.Api/TestResults"

dotnet test "MY.dockerlearning.sln" --logger "trx" --collect:"Code Coverage" --results-directory "App/dockerlearning.Api/TestResults" /p:CoverletOutput="App/dockerlearning.Api/TestResults/dockerlearningSystem_CoverageReport.xml" /p:CoverletOutputFormat="cobertura"


================================== Local SonarQube========================

sqp_6de610571ea573e99ff869e54c5423125d21c063
dockerlearningservicelxpoc
sonar.host.url="https://0ea5-2409-40e3-30-60d5-9535-a6d1-8367-a79b.ngrok-free.app"

docker build --no-cache --progress plain --build-arg sonar_token="sqp_6de610571ea573e99ff869e54c5423125d21c063" --build-arg sonar_branch="develop" --build-arg sonar_qualitygate=true --build-arg build_env_name=dev -t dockerlearningservicelxpoc:1.0.0 -f ./App/dockerlearning.Api/Dockerfile .


docker run --name dockerlearningServiceLinuxApi -d -p 8085:5000 dockerlearningservicelxpoc:1.0.0









