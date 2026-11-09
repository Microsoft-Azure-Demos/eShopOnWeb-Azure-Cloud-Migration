# Architectural Decisions – eShopOnWeb Azure Migration

## 1. Purpose

This document records the main architectural decisions made during the migration of the eShopOnWeb application from a standalone Docker-based deployment to Microsoft Azure.

The objective is to explain not only which Azure services were selected, but also why they were chosen, which alternatives were considered, and what trade-offs these decisions introduce.

The decisions reflect the scope of this project: demonstrating a practical migration of an existing containerized ASP.NET Core application to Azure managed services without introducing unnecessary infrastructure complexity.

---

## 2. Compute and Application Hosting

### Decision: Azure App Service instead of Azure Container Apps

**Selected service:** Azure App Service (Linux, custom containers)

The MVC frontend and Public API are deployed as separate Linux App Services, using Docker images published to Docker Hub.

#### Why Azure App Service?

Azure App Service provides a managed hosting platform for web applications and APIs. It supports custom Linux containers, HTTPS, deployment configuration, application settings, logging integration and platform-managed infrastructure.

For this migration, App Service offered a straightforward way to move the existing Dockerized ASP.NET Core applications to Azure without having to manage the underlying compute infrastructure.

The application already had two independently deployable components:

- MVC frontend
- Public API

Each component could therefore be deployed as its own App Service while sharing the same App Service Plan.

#### Why not Azure Container Apps?

Azure Container Apps is also a suitable platform for hosting containerized applications. It offers capabilities particularly useful for microservice-oriented and event-driven architectures.

These include:

- Independent scaling of container apps.
- Scale-to-zero capabilities, depending on configuration.
- KEDA-based event-driven scaling.
- Revisions and traffic splitting.
- Built-in ingress and service discovery.
- Integration with Dapr and other distributed application capabilities.

However, this project did not require most of these capabilities.

The application consists of two conventional ASP.NET Core web workloads. It does not currently require event-driven scaling, scale-to-zero, Dapr, or a more elaborate microservices platform.

App Service therefore provided a simpler operational model for the requirements of this migration.

#### Comparison

| Consideration | Azure App Service | Azure Container Apps |
|---|---|---|
| Primary use case | Managed web applications and APIs | Containerized applications and microservices |
| Container support | Custom Linux containers | Native container workloads |
| Infrastructure management | Fully managed platform | Fully managed platform |
| Scaling | Manual and autoscaling options | Autoscaling, including event-driven scaling |
| Scale to zero | Not the typical App Service model | Supported |
| Revisions and traffic splitting | Deployment slots and deployment options | Native revisions and traffic splitting |
| Microservices capabilities | Possible, but less specialized | Stronger built-in support |
| Operational complexity | Relatively straightforward | More platform concepts to configure |
| Fit for this project | Selected | Valid alternative, not required |

#### Trade-offs

The App Service approach keeps the deployment relatively simple, but it does not provide all the container-native scaling and traffic-management capabilities available in Container Apps.

Both applications currently share one App Service Plan. Consequently, they share the underlying plan capacity. Independent application deployment does not mean fully independent compute capacity.

If the project evolves into a larger microservices architecture, requires event-driven scaling, or needs scale-to-zero, Azure Container Apps would be worth reconsidering.

**Conclusion:** Azure App Service was selected because it provides the required managed container hosting with a simpler operational model and without introducing platform capabilities that the application does not currently need.

---

## 3. Database Hosting

### Decision: Azure SQL Database instead of a SQL Server container

**Selected service:** Azure SQL Database

The original standalone deployment used SQL Server running in a Docker container. For the Azure deployment, the database layer was moved to Azure SQL Database.

#### Why Azure SQL Database?

Using a managed database service removes the need to operate SQL Server infrastructure ourselves.

Azure handles important database platform responsibilities, including infrastructure maintenance, patching and built-in availability capabilities, depending on the selected service tier and configuration.

The application requires relational database functionality and Entity Framework Core integration. Azure SQL Database provides the required SQL Server-compatible database platform.

Two separate databases were provisioned:

- `eshop-catalog`
- `eshop-identity`

#### Alternatives considered

- SQL Server running in an Azure VM.
- SQL Server running in a container.
- Azure SQL Managed Instance.

A SQL Server VM or container would provide greater control over the database environment, but would also introduce additional infrastructure and maintenance responsibilities.

Azure SQL Managed Instance provides broader SQL Server compatibility, but its capabilities were not necessary for this application.

**Conclusion:** Azure SQL Database was selected to reduce database infrastructure management while retaining the relational database capabilities required by the application.

---

## 4. Database Authentication

### Decision: Microsoft Entra Managed Identity instead of SQL authentication

**Selected mechanism:** System-assigned Managed Identity

Both App Services use system-assigned Managed Identities to authenticate to Azure SQL Database.

#### Why Managed Identity?

Traditional SQL authentication requires a username and password to be stored and managed.

Managed Identity allows an Azure-hosted application to obtain an identity-backed access token without storing database credentials in application configuration.

The identities were created automatically by Azure when Managed Identity was enabled on each App Service.

Corresponding database users were created and granted the required permissions.

This provides:

- Passwordless authentication.
- No long-lived database passwords in application settings.
- Identity-based access control.
- Integration with Microsoft Entra ID.
- Reduced credential-management overhead.

#### Why system-assigned instead of user-assigned identity?

A system-assigned identity is tied to the lifecycle of its Azure resource.

For this project, each App Service has its own identity, which keeps the access relationship straightforward.

A user-assigned identity would be useful if the same identity needed to be shared across multiple Azure resources or retained independently of an individual App Service lifecycle.

**Conclusion:** System-assigned Managed Identity was selected because each application has its own database access identity and does not require a shared identity lifecycle.

---

## 5. Static Asset Storage

### Decision: Azure Blob Storage instead of container-local files

**Selected service:** Azure Blob Storage

Product images, logos, banners and other static image assets were migrated from the application's local `wwwroot/images` directory to Azure Blob Storage.

#### Why Blob Storage?

Container filesystems should not be treated as durable shared storage for application assets.

When containers are replaced or redeployed, locally stored files may not be preserved. Furthermore, separate application instances do not automatically share their local filesystems.

Blob Storage provides durable object storage that is independent of the application container lifecycle.

Benefits include:

- Independent storage lifecycle.
- Centralized static asset storage.
- Reduced dependency on the application container filesystem.
- Ability to serve assets directly from storage.
- Easier separation of application deployment and static content management.

The application uses the `ImageBaseUrl` configuration setting to construct external image URLs.

#### Alternatives considered

- Keeping images inside the Docker image.
- Azure Files.
- Azure Blob Storage with Azure CDN or Azure Front Door.

Keeping images inside the Docker image would work for immutable assets, but would couple content updates to application image rebuilds.

Azure Files is more appropriate when applications require shared filesystem semantics.

For object-based static assets, Blob Storage is the more natural choice.

**Conclusion:** Blob Storage was selected because the application primarily needs durable storage and HTTP access to static image assets, rather than a shared filesystem.

---

## 6. Application Configuration

### Decision: Azure App Service Application Settings instead of hardcoded environment-specific values

Application configuration is externalized through Azure App Service Application Settings and connection configuration.

This separates application code and container images from environment-specific values.

The same container image can therefore be deployed into different environments with different configuration values.

Benefits include:

- Environment-independent Docker images.
- Easier configuration changes.
- Reduced risk of embedding environment-specific values in source code.
- Better separation between application code and deployment configuration.

Sensitive values should be managed through an appropriate secret-management solution, such as Azure Key Vault, rather than treated as ordinary application settings.

**Conclusion:** Externalized configuration was selected to make the application easier to deploy and maintain across environments.

---

## 7. Container Image Distribution

### Decision: Docker Hub instead of Azure Container Registry

**Selected registry:** Docker Hub

The Azure-specific MVC and Public API images are published to Docker Hub.

This provides a straightforward image distribution mechanism for the project.

Azure Container Registry would be a natural alternative, especially for a production Azure environment.

ACR provides Azure integration, private image storage, role-based access control and managed identity-based authentication for supported deployment scenarios.

For this project, Docker Hub was sufficient for distributing the two application images.

**Conclusion:** Docker Hub was retained to keep the container publishing workflow straightforward. Azure Container Registry could be introduced as a future improvement to centralize container image management within Azure.

---

## 8. Shared App Service Plan

### Decision: Deploy both applications on the same App Service Plan

The MVC frontend and Public API are hosted in separate App Services but share the same App Service Plan.

This reduces the number of compute resources required for the demonstration and allows both applications to use the same underlying plan capacity.

#### Trade-offs

Although the applications are deployed independently, they share the plan's compute resources.

Consequently:

- Resource consumption by one application can affect the other.
- Both applications share the plan's scaling configuration.
- They do not have fully independent compute capacity.

For production workloads with different performance requirements, separate App Service Plans could provide stronger resource isolation and independent scaling.

**Conclusion:** A shared App Service Plan was selected as a cost-conscious and operationally simple choice for this project.

---

## 9. Future Architectural Considerations

The following services and capabilities could be introduced as the project evolves:

| Area | Potential Azure Service | Purpose |
|---|---|---|
| Secret management | Azure Key Vault | Centralized management of secrets and sensitive configuration |
| Monitoring | Azure Application Insights | Application telemetry, tracing and performance monitoring |
| Centralized logging | Azure Monitor / Log Analytics | Centralized log collection and analysis |
| Infrastructure as Code | Terraform or Bicep | Repeatable infrastructure provisioning |
| CI/CD | GitHub Actions with OIDC | Automated build, test and deployment |
| Container registry | Azure Container Registry | Private Azure-integrated image storage |
| Network security | Private Endpoints and VNet integration | Restrict access to selected Azure services |
| Static asset delivery | Azure Front Door or Azure CDN | Global delivery and caching of static content |
| Data Protection | Azure Blob Storage | Persistent ASP.NET Core Data Protection keys, if required |
| Compute alternative | Azure Container Apps | Event-driven scaling and container-native application hosting |

---

## 10. Overall Architectural Assessment

The resulting architecture uses managed Azure services for application hosting, database persistence, identity-based authentication and static asset storage.

The main design principle was to use managed services where they provide clear operational benefits, while avoiding unnecessary platform complexity.

The selected architecture is appropriate for demonstrating a practical migration of an existing ASP.NET Core application to Azure.

It is not intended to represent a fully production-hardened or highly available enterprise architecture. Further improvements in monitoring, network security, secret management, deployment automation and infrastructure provisioning remain possible.