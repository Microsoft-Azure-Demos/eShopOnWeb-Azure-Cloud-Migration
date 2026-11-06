# eShopOnWeb – Containerized Application

A containerized e-commerce application based on [NimblePros/eShopOnWeb](https://github.com/NimblePros/eShopOnWeb).

This project demonstrates how to package a multi-service .NET application using Docker and Docker Compose, manage configuration through environment variables, persist database data using Docker volumes, and prepare reusable images for deployment to cloud platforms such as Microsoft Azure.

## Architecture

The application consists of four Docker services:

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
                    |
                    v
             Public API :5200
                    |
                    v
              SQL Server
                 :1433

          Web MVC / Public API
                    |
                    v
                  Seq
                 :5341
```

All services communicate through the Docker Compose network.

## Technologies

- .NET 10
- ASP.NET Core
- Docker
- Docker Compose
- Microsoft SQL Server 2022
- Seq
- Git and GitHub
- Docker Hub

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

## Configuration

The application uses environment variables for local configuration.

Create your local environment file:

**PowerShell:**

```powershell
Copy-Item .env.example .env
```

Update `.env` with your own passwords and configuration.

The `.env` file is excluded from Git to prevent credentials from being committed.

## Running the application

### 1. Build the application images

```powershell
docker compose build
```

### 2. Start all services

```powershell
docker compose up -d
```

### 3. Check service status

```powershell
docker compose ps
```

### 4. Access the application

- Web application: http://localhost:5106
- Public API: http://localhost:5200
- Seq dashboard: http://localhost:8081

### 5. Stop the application

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

**Important:** Avoid `docker compose down -v` unless you intentionally want to delete the persistent data.

## Container images

The project maintains two application images:

| Image | Description |
|---|---|
| `eshopwebmvc` | E-commerce frontend |
| `eshoppublicapi` | Backend API |

SQL Server and Seq use their respective official images.

### Build images locally

```powershell
docker compose build eshopwebmvc eshoppublicapi
```

### Docker Hub publication

The application images will be published to Docker Hub using versioned tags.

Planned image references:

```text
<DOCKERHUB_USERNAME>/eshopwebmvc:1.0.0
<DOCKERHUB_USERNAME>/eshoppublicapi:1.0.0
```

Once published, these images can be pulled and reused on other Docker hosts without rebuilding the application from source.

## Useful Docker commands

```powershell
# List running containers
docker compose ps

# View application logs
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