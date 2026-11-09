# eShopOnWeb – Azure Cloud Migration

## Overview

This project demonstrates the migration of an existing ASP.NET Core e-commerce application from a standalone/on-premises deployment model to Microsoft Azure.

The original application is based on [NimblePros/eShopOnWeb](https://github.com/NimblePros/eShopOnWeb) and was initially configured to run locally using Docker Compose, with application services and SQL Server hosted in containers.

The objective of this project was to migrate the application to Azure while adapting its deployment architecture, application configuration, database connectivity, storage and security to take advantage of managed cloud services.

This is not simply a container deployment. It demonstrates the practical steps involved in refactoring and modernizing an existing application for cloud hosting.

## Migration Objectives

- Migrate the application from standalone Docker Compose to Azure App Service.
- Adapt the original deployment architecture to use two independently hosted App Services.
- Replace containerized SQL Server with Azure SQL Database.
- Implement passwordless database authentication using Microsoft Entra Managed Identity.
- Migrate product images and other static assets to Azure Blob Storage.
- Separate cloud-specific application configuration from local development configuration.
- Parameterize the application using Azure App Service application settings and environment variables.
- Publish dedicated Azure container images to Docker Hub.
- Deploy and validate the application in Azure.

---

## Architecture

### Azure Architecture

| Component | Azure Service | Purpose |
|---|---|---|
| Web MVC | Azure App Service (Linux) | Hosts the frontend |
| Public API | Azure App Service (Linux) | Hosts the REST API |
| Catalog Database | Azure SQL Database | Stores catalog data |
| Identity Database | Azure SQL Database | Stores identity and authentication data |
| Static Assets | Azure Blob Storage | Stores product images, logos, banners and other images |
| Application Identity | Microsoft Entra Managed Identity | Passwordless database authentication |
| Container Registry | Docker Hub | Stores Azure-specific application images |
| Application Configuration | App Service Application Settings | Provides environment-specific configuration |

### Architecture Diagram

*Architecture diagram to be added.*

<!-- TODO: Add Azure architecture diagram here. -->

The MVC application and Public API are deployed independently as Docker containers on Azure App Service. Both use Azure SQL Database for persistence and Managed Identity for database authentication.

Both App Services run Linux containers on the same App Service Plan.

### Azure Resources

| Resource | Name |
|---|---|
| Resource Group | `app-service-demo` |
| App Service Plan | `ASP-appservicedemo-9ee0` |
| MVC App Service | `app-service-webmvc` |
| Public API App Service | `app-service-publicapi` |
| Azure SQL Server | `egamor-sqlserver` |
| Catalog Database | `eshop-catalog` |
| Identity Database | `eshop-identity` |
| Storage Account | `egamoreshopimages` |
| Blob Container | `images` |

---

## Migration: On-Premises to Azure

The original standalone deployment architecture was adapted to use Azure managed services.

| Area | Standalone Deployment | Azure Cloud Deployment |
|---|---|---|
| Application hosting | Docker containers | Azure App Service |
| Application architecture | Docker Compose | Two independent App Services |
| Database | SQL Server container | Azure SQL Database |
| Database authentication | SQL authentication | Managed Identity |
| Database connectivity | Docker internal network | Azure SQL endpoint |
| Static assets | Local application filesystem | Azure Blob Storage |
| Application configuration | Environment variables and `.env` | App Service Application Settings |
| Container images | Standalone Docker images | Azure-specific Docker images |
| Logging | Seq container | Azure application logging |

---

## Migration and Implementation Details

### 1. Application Containerization

The existing ASP.NET Core application was containerized using Docker.

Dedicated Docker images were created for the MVC frontend and Public API, allowing the application components to be deployed independently.

### 2. Migration to Azure App Service

The original Docker Compose-based deployment architecture was adapted to use two Azure Linux App Services:

- **MVC frontend:** `app-service-webmvc`
- **Public API:** `app-service-publicapi`

Both applications run as Linux containers on the same Azure App Service Plan.

This separates the frontend and API hosting responsibilities while allowing each application component to be deployed independently.

### 3. Application Configuration and Parameterization

The application was refactored to externalize environment-specific configuration.

Azure App Service application settings and connection configuration provide deployment-specific values at runtime.

This approach avoids embedding environment-specific configuration directly into the Docker images.

The same application images can therefore be deployed with different configuration values across environments without rebuilding the images.

### 4. Database Migration to Azure SQL

The application database layer was adapted to use Azure SQL Database instead of the original containerized SQL Server deployment.

Two databases were provisioned:

- `eshop-catalog` – catalog data.
- `eshop-identity` – identity and authentication data.

The application uses Azure SQL connectivity with Microsoft Entra ID authentication through Managed Identity.

### 5. Static Asset Migration to Azure Blob Storage

Static image assets were migrated from the application's local `wwwroot/images` directory to Azure Blob Storage.

Migrated assets include:

- Product images.
- Brand logos.
- Main catalog banners.
- Shopping cart icons.
- Navigation arrows.
- Login and logout icons.
- Other application images.

**Blob Storage configuration:**

```text
Storage Account: egamoreshopimages
Container:       images
Base URL:        https://egamoreshopimages.blob.core.windows.net
```

The application was refactored to retrieve images from Blob Storage instead of relying on local image files.

The `ImageBaseUrl` setting is configured through Azure App Service application settings, allowing the image storage endpoint to be changed without modifying the Docker images.

Both Razor views and CSS files were updated to use the external image locations.

### 6. Managed Identity and Passwordless Database Authentication

System-assigned Managed Identities were enabled for both App Services.

These identities were granted access to the Azure SQL databases through Microsoft Entra ID.

Database users were created for the application identities and assigned the required permissions:

- `db_datareader`
- `db_datawriter`
- `db_ddladmin`

Connection strings were configured to use Managed Identity authentication.

This eliminates the need to store database usernames and passwords in the application configuration and demonstrates identity-based access to Azure resources without relying on long-lived database credentials.

### 7. Application Security Configuration

JWT signing and validation were configured consistently between the MVC application and Public API.

Cloud-specific application settings, including authentication-related configuration, are provided through Azure App Service rather than being embedded in the container images.

---

## Azure Container Images

Dedicated Docker Hub images are maintained for the Azure deployment.

| Application | Docker Hub Image |
|---|---|
| Web MVC | `egamor/eshopwebmvc-azure:1.0.0` |
| Public API | `egamor/eshoppublicapi-azure:1.0.0` |

---

## Deployment Validation

The migration was validated through the following checks:

- [x] MVC application successfully deployed and accessible.
- [x] Public API successfully deployed and accessible.
- [x] Swagger documentation available through the Public API.
- [x] Successful connectivity to both Azure SQL databases.
- [x] Application catalog and product images successfully retrieved from Blob Storage.
- [x] Static application assets successfully loaded from Blob Storage.
- [x] Basket and checkout pages displaying their migrated images.
- [x] Application functionality and purchase workflow tested.

---

## Azure Application URLs

| Application | URL |
|---|---|
| Web MVC | [Open Web MVC Application](https://app-service-webmvc-fkg2bnfphedtarah.westus3-01.azurewebsites.net) |
| Public API Swagger | [Open Swagger UI](https://app-service-publicapi-e6hdekhtghdmauek.westus3-01.azurewebsites.net/swagger/index.html) |

---

## Future Improvements

The following enhancements are planned to improve the deployment's operational readiness, security and automation:

- Centralized application monitoring with Azure Application Insights.
- Persistent ASP.NET Core Data Protection keys using Azure Blob Storage, if required.
- Improved network security using private endpoints and restricted SQL access.
- Azure Key Vault integration for centralized secret management.
- Automated CI/CD deployment using GitHub Actions and OIDC.
- Infrastructure as Code using Terraform or Bicep.
- Dedicated database migration deployment process.
- Production-ready logging, monitoring and alerting.

---

## Architectural Decisions

The reasoning behind the selected Azure services, architectural alternatives and associated trade-offs is documented separately.

See [Architectural Decisions](./Documentation/architectural-decisions.md).

---

## Related Project

**Standalone / On-Premises Deployment:**

[Microsoft-Azure-Demos/eShopOnWeb-Containerized-Application](https://github.com/Microsoft-Azure-Demos/eShopOnWeb-Containerized-Application)

---

## Project Purpose

This repository serves as a practical demonstration of migrating an existing containerized ASP.NET Core application to Microsoft Azure.

Rather than simply deploying existing Docker images to the cloud, the project demonstrates the architectural and application-level changes required to adapt an application for Azure hosting, managed database services, cloud storage, externalized configuration and identity-based security.

It is intended as a hands-on Cloud Engineering and DevOps portfolio project.

---

## License

Refer to the original [NimblePros/eShopOnWeb](https://github.com/NimblePros/eShopOnWeb) project and its licensing terms.
