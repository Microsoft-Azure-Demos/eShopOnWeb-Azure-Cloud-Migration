# eShopOnWeb – Azure Cloud Migration

## Overview

This project demonstrates the migration of a containerized e-commerce application from a standalone/on-premises deployment model to Microsoft Azure.

The original application, based on [NimblePros/eShopOnWeb](https://github.com/NimblePros/eShopOnWeb), was initially configured to run locally using Docker Compose, with application services and SQL Server hosted in containers.

The objective of this project was to migrate the application to Azure while adapting its configuration, database connectivity, security and deployment architecture to use managed cloud services.

This is not simply a container deployment. It demonstrates the practical steps involved in modernizing an existing application for the cloud.

## Migration Objectives

- Migrate the application from standalone Docker Compose to Azure App Service.
- Replace containerized SQL Server with Azure SQL Database.
- Implement passwordless database authentication using Microsoft Entra Managed Identity.
- Separate cloud-specific application configuration from local development configuration.
- Publish dedicated Azure container images to Docker Hub.
- Deploy and validate the complete application in Azure.

## Architecture

### Azure Architecture

| Component | Azure Service | Purpose |
|---|---|---|
| Web MVC | Azure App Service (Linux) | Hosts the frontend |
| Public API | Azure App Service (Linux) | Hosts the REST API |
| Catalog Database | Azure SQL Database | Stores catalog data |
| Identity Database | Azure SQL Database | Stores identity and authentication data |
| Application Identity | Microsoft Entra Managed Identity | Passwordless database authentication |
| Container Registry | Docker Hub | Stores Azure-specific application images |

### Architecture Diagram

[Architecture diagram to be added]

## Technology Stack

- **Application:** ASP.NET Core, C#
- **Containers:** Docker
- **Cloud Platform:** Microsoft Azure
- **Compute:** Azure App Service (Linux containers)
- **Database:** Azure SQL Database
- **Identity:** Microsoft Entra ID, Managed Identity
- **Container Registry:** Docker Hub
- **API Documentation:** Swagger / OpenAPI

## Migration: On-Premises to Azure

| Area | Standalone Deployment | Azure Cloud Deployment |
|---|---|---|
| Application hosting | Docker containers | Azure App Service |
| Database | SQL Server container | Azure SQL Database |
| Database authentication | SQL authentication | Managed Identity |
| Database connectivity | Docker internal network | Azure SQL endpoint |
| Application configuration | Environment variables and `.env` | App Service Application Settings |
| Container images | Standalone Docker images | Azure-specific Docker images |
| Logging | Seq container | Azure application logging |

## Azure Container Images

Dedicated Docker Hub repositories are maintained for the Azure deployment.

| Application | Docker Hub Image |
|---|---|
| Web MVC | `egamor/eshopwebmvc-azure:1.0.0` |
| Public API | `egamor/eshoppublicapi-azure:1.0.0` |

## Security and Identity

The application uses system-assigned Managed Identities for database connectivity.

Key implementation details:

- Enabled system-assigned Managed Identity on both App Services.
- Created corresponding database users in Azure SQL Database.
- Assigned database permissions to the application identities.
- Configured connection strings to use Active Directory Managed Identity authentication.
- Configured JWT signing and validation consistently between MVC and Public API.
- Stored cloud-specific application settings in Azure App Service rather than embedding secrets in container images.

## Deployment Validation

The migration was validated through the following tests:

- Web MVC application successfully deployed and accessible.
- Public API successfully deployed and accessible.
- Swagger documentation available through the Public API.
- Successful connectivity to both Azure SQL databases.
- Successful end-to-end purchase workflow.

## Azure Application URLs

- **Web MVC:** https://app-service-webmvc-fkg2bnfphedtarah.westus3-01.azurewebsites.net
- **Public API Swagger:** https://app-service-publicapi-e6hdekhtghdmauek.westus3-01.azurewebsites.net/swagger/index.html

## Future Improvements

- Centralized application monitoring with Azure Application Insights.
- Persistent ASP.NET Core Data Protection keys.
- Improved network security using private endpoints and restricted SQL access.
- Automated CI/CD deployment using GitHub Actions and OIDC.
- Infrastructure as Code using Terraform or Bicep.
- Dedicated database migration deployment process.
- Production-ready logging, monitoring and alerting.

## Related Project

**Standalone / On-Premises Deployment:**  
[(https://github.com/Microsoft-Azure-Demos/eShopOnWeb-Containerized-Application)]


## License

Refer to the original eShopOnWeb project and its licensing terms.