<h1 align="center">The Adventure Junkie Web API</h1>

## 1.	Introduction
To extend *The Adventure Junkie* universe showcased in my developer portfolio, I’ve implemented a **RESTful ASP.NET Core Web API**, based on the existing *The Adventure Junkie* Android app and ASP.NET Core MVC website.

The API supports all major HTTP methods and includes best practices such as **versioning**, **searching**, **filtering**, **pagination**, **query fields**, and **parameter handling** — enabling consistency across platforms.

The API is deployed via **GitHub Actions** and documented via **Swagger UI**, which is routed through an **Azure API Management (APIM)** instance. This setup allows for enhanced security and cost management by enforcing:
- ✅ Rate Limiting  
- ✅ Quotas  
- ✅ Redis-based Caching (Cache-Aside Pattern)

> ⚠️ **Note**: The hosted API does not respond to direct requests.  
> Access is only available via the Swagger UI and the **Postman collection** included in the repository, which queries the APIM instance.  
> Subscription keys are **not required** to simplify the testing process, by removing the hassle of signing up in the developer portal.

The API is hosted independently in an **Azure App Service**, using a separate codebase but sharing the same **Azure SQL Database** as the MVC website. The database access layer was scaffolded using **Entity Framework Core**. It was a concious design decision to enable **independent scalability** of the Web API and the website.

🔍 If you want to **view your changes in real time**, simply visit The Adventure Junkie website for a live UI reflection of API operations:<br> [The Adventure Junkie Website](https://theadventurejunkie.azurewebsites.net)

---

## 2.	Technologies Used
•	Web Application: <br>
🔹ASP.NET Core MC <br>
🔹	Entity Framework Core (Scaffold an existing Database) <br>
🔹	AutoMapper (DTO conversion to Entities and vice versa) <br>
🔹	Swagger UI (API Testing & Documentation) <br>
<br>
•	Infrastructure: <br>
🔹	GitHub Action (CI/CD with workflow_dispatch Trigger and Service Principal for Authentication) <br>
🔹	Azure API Management Instance (Rate Limiting, CORS, Quota and Set Header Policies) <br>
🔹	Azure Cache For Redis (Cache-Aside Pattern) <br>
🔹	Application Insights (Auto Instrumented) <br>
🔹	Postman (API testing) <br>
🔹	Azure App Services (F1 Plan) <br>
<br>
•	Database: <br>
🔹	Azure SQL Database (Managed Instance) <br>
🔹	Microsoft SQL Server Management Studio <br>

## 3.	Code

### 🧱 Architecture Patterns
- 🔁 **Repository Pattern**: Handles all data access logic.
- 🧩 **Service Layer**: Abstracts caching logic and other business operations.
- 🎯 **Interfaces**: Used throughout for decoupling and easier testing.
  
![image](https://github.com/user-attachments/assets/240a8ad8-6aec-4e68-b10c-d57ad421211b)

### 🧾 API Documentation
- 🧪 Swagger UI: Auto-generated with detailed XML comments.
- 🔄 DTOs & AutoMapper:
  - Clean conversion between entities and DTOs.
  - Profiles defined for automated mappings.

### 🚀 CI/CD Pipeline
- ⚙️ GitHub Actions:
  - CI/CD with workflow_dispatch trigger.
  - Uses Azure Service Principal for deployment.
- 📦 Builds and deploys to Azure App Services.

### 🔐 Middleware & API Management (APIM)
- 🛡️ Custom middleware:
  - Serves Swagger UI through Azure API Management.
  - Enforces request validation to prevent direct API access.
- 🧰 APIM Policies:
  - ✅ Rate Limiting (429)
  - ✅ Quotas (403)
  - 🔄 CORS and header modification
- 🔓 No Subscription Key Required:
  - Simplifies testing for external users.

![image](https://github.com/user-attachments/assets/c01f6bb7-53ab-4833-854a-12345d7393b8)

### 🧊 Caching Layer (Redis)
- 🚀 Azure Cache for Redis:
  - Implements the Cache-Aside Pattern.
  - ⏳ 6-hour absolute expiration
  - 🔁 1-hour sliding expiration window
- 🔄 Cache updated on upsert operations (create/update).<br>
>💡 While in-memory or APIM-level caching would suffice, Redis was chosen to simulate enterprise-grade architecture.


![image](https://github.com/user-attachments/assets/b2d0f624-1639-435c-82a2-15a79f1aa6e4)

### 🗃️ Database & Observability
- 🧱 Entity Framework Core:
  - Scaffolded from existing Azure SQL database.
  - Schema changes applied using migrations.
  - Query logic implemented via LINQ.
- 📈 Application Insights:
  - Auto-instrumented for end-to-end monitoring.
  - Logs, traces, metrics, and exception handling built-in.

## 4.	Infrastructure Implementation
The overall architecture is provided below to get a brief overview of the Implementation:<br>
<br>
![image](https://github.com/user-attachments/assets/42fc9e9b-4572-49d5-985b-aa5b77f8c6d0)
<br>
 
1. **API Management (APIM)** acts as the gateway to the API and is accessed via **Postman**, **cURL**, or **Swagger UI**. Inbound policies are applied to validate headers and usage (e.g., quotas, rate limits).
2. If the request is authorized, it is forwarded to the hosted **Azure App Service**.
3. The App Service calls the **Redis Cache** (via the Service Layer) in an attempt to serve the response from memory.
4. If the cache contains the requested data (Cache Hit), it is returned immediately.
5. If there is a **Cache Miss**, the App Service queries the **Azure SQL Database** through the Repository Layer.
6. Once the data is retrieved from the database, the App Service updates the **Redis Cache** and continues processing.
7. After applying all business logic and performing DTO transformations, the formatted response is returned to the client.
8. **Application Insights** continuously logs telemetry for request tracing, performance monitoring, and fault detection.

