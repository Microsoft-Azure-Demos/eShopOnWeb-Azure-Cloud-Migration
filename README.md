# eShopOnWeb – Containerized Application

A containerized e-commerce application based on [NimblePros/eShopOnWeb](https://github.com/NimblePros/eShopOnWeb).

This project demonstrates how to containerize a multi-service .NET application using Docker, publish reusable application images to Docker Hub, orchestrate services with Docker Compose, manage configuration through environment variables, and persist database data using Docker volumes.

The project also serves as a foundation for a cloud deployment on Microsoft Azure.

## Architecture

The application consists of four Docker services, orchestrated through a single Docker Compose configuration.

| Service | Technology | Purpose | Local access |
|---|---|---|---|
| Web MVC | ASP.NET Core | E-commerce frontend | http://localhost:5106 |
| Public API | ASP.NET Core | Backend API | http://localhost:5200 |
| Database | Microsoft SQL Server 2022 | Application data | localhost:1433 |
| Logging | Seq | Centralized application logs | http://localhost:8081 |

### Architecture overview

```text
                    Browser
                       |
                       v
                Web MVC :5106
                Docker Image
                       |
                       v
                Public API :5200
                Docker Image
                       |
                       v
                SQL Server :1433
                Persistent Volume

             Web MVC / Public API
                       |
                       v
                    Seq :5341
                Persistent Volume
```

All services communicate through a dedicated Docker Compose network.

The Web MVC and Public API services use separately published Docker images. Docker Compose manages their deployment, networking, configuration, and dependencies.

## Technologies

- .NET 10
- ASP.NET Core
- Docker
- Docker Compose
- Docker Hub
- Microsoft SQL Server 2022
- Seq
- Git and GitHub

## Project structure

```text
eShop/
├── src/
│   ├── Web/
│   └── PublicApi/
├── tests/
├── infra/
├── docker-compose.yml
├── docker-compose.override.yml
├── .env.example
├── .gitignore
└── README.md
```

## Prerequisites

- Docker Desktop
- Docker Compose
- Git

You do not need the .NET SDK to run the published Docker images.

## Docker Hub images

The project maintains two reusable application images:

| Service | Docker Hub image |
|---|---|
| Web MVC | [egamor/eshopwebmvc](https://hub.docker.com/r/egamor/eshopwebmvc) |
| Public API | [egamor/eshoppublicapi](https://hub.docker.com/r/egamor/eshoppublicapi) |

Both images are published with versioned tags:

```text
egamor/eshopwebmvc:1.0.0
egamor/eshoppublicapi:1.0.0
```

The `latest` tag is also available for both images.

SQL Server and Seq use their respective publisher-provided images.

## Configuration

The application uses environment variables for local configuration.

Create your local environment file:

**PowerShell:**

```powershell
Copy-Item .env.example .env
```

Update `.env` with your own passwords and configuration.

The `.env` file is excluded from Git to prevent credentials from being committed. Only `.env.example`, containing placeholder values, is committed.

## Running the application

### 1. Clone the repository

```powershell
git clone <YOUR_GITHUB_REPOSITORY_URL>
cd <YOUR_REPOSITORY_FOLDER>
```

### 2. Configure environment variables

```powershell
Copy-Item .env.example .env
```

Edit `.env` and provide your own configuration.

### 3. Pull the application images

```powershell
docker compose pull
```

Docker Compose downloads the Web MVC and Public API images from Docker Hub.

### 4. Start all services

```powershell
docker compose up -d
```

Docker Compose starts all four services and connects them through the project network.

### 5. Check service status

```powershell
docker compose ps
```

### 6. Access the application

- Web application: http://localhost:5106
- Public API: http://localhost:5200
- Seq dashboard: http://localhost:8081

### 7. Stop the application

```powershell
docker compose down
```

This stops and removes the containers but preserves named volumes.

## Persistent storage

SQL Server and Seq use Docker named volumes to preserve data independently of container lifecycles.

| Volume | Purpose |
|---|---|
| `eshop_sqlserver-data` | SQL Server database files |
| `eshop_seq-data` | Seq persistent data |

List volumes:

```powershell
docker volume ls
```

Inspect SQL Server volume:

```powershell
docker volume inspect eshop_sqlserver-data
```

**Important:** Avoid `docker compose down -v` unless you intentionally want to delete persistent data.

## Useful Docker commands

```powershell
# List running containers
docker compose ps

# Pull the latest configured images
docker compose pull

# Start services
docker compose up -d

# View all service logs
docker compose logs -f

# View individual service logs
docker compose logs -f eshopwebmvc
docker compose logs -f eshoppublicapi

# Restart services
docker compose restart

# List local images
docker images

# List persistent volumes
docker volume ls
```

## Image versioning and updates

The application images are independently versioned and published to Docker Hub.

To deploy a newer application version:

1. Update the image tag in `docker-compose.yml`.
2. Pull the updated images.
3. Recreate the containers.

```powershell
docker compose pull
docker compose up -d
```

This allows the application to be deployed on another Docker host without rebuilding the source code.

## Future cloud deployment

The next development phase is to deploy the application to Microsoft Azure.

Planned components include:

- Azure Container Registry or Docker Hub for container images.
- Azure networking and private connectivity.
- Azure-hosted application services.
- Managed database services.
- Terraform for Infrastructure as Code.
- GitHub Actions for CI/CD automation.

## Acknowledgements

This project is based on Microsoft's eShopOnWeb reference application, maintained by [NimblePros](https://github.com/NimblePros/eShopOnWeb).

The original application serves as the foundation, while this project focuses on containerization, deployment automation, and cloud infrastructure.

Please retain the original project's license and copyright notices.