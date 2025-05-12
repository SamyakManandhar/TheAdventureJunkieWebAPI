<h1 align="center">The Adventure Junkie Web API</h1>

## 1.	Introduction
To extend *The Adventure Junkie* universe showcased in my developer portfolio, I’ve implemented a **RESTful ASP.NET Core Web API**, based on the existing *Adventure Junkie* Android app and ASP.NET Core MVC website.

The API supports all major HTTP methods and includes best practices such as **versioning**, **searching**, **filtering**, **pagination**, **query fields**, and **parameter handling** — enabling consistency across platforms.

The API is documented and testable via **Swagger UI**, which is routed through an **Azure API Management (APIM)** instance. This setup allows for enhanced security and cost management by enforcing:
- ✅ Rate Limiting  
- ✅ Quotas  
- ✅ Redis-based Caching (Cache-Aside Pattern)

> ⚠️ **Note**: The hosted API does not respond to direct requests.  
> Access is only available via the Swagger UI and the **Postman collection** included in the repository, which queries the APIM instance.  
> Subscription keys are **not required** to simplify the testing process, by removing the hassle of signing up in the developer portal.

The API is hosted independently in an **Azure App Service**, using a separate codebase but sharing the same **Azure SQL Database** as the MVC website. The database access layer was scaffolded using **Entity Framework Core**. It was a concious design decision to enable **independent scalability** of the Web API and the website.

🔍 If you want to **view your changes in real time**, simply visit the Adventure Junkie website for a live UI reflection of API operations.

---

## 2.	Technologies Used
•	Web Application: <br>
🔹ASP.NET Core MC <br>
🔹	Entity Framework Core (Scaffold an existing Database) <br>
🔹	AutoMapper (DTO conversion to Entities and vice-versa) <br>
🔹	Swagger UI (API Testing & Documentation) <br>

<br>
•	Infrastructure: <br>
🔹	GitHub Action (CI/CD with Workflow Dispatch Trigger and Service Principal for Authentication) <br>
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
The code follows the Model-Controller Imoplementation of ASP.NET Core, leveraging the repository pattern to access database and service layer to abstract away the caching logic. XML comments are added to genereated swagger UI to enable for more thorough documenatation. DTO classes and Profiles are introduced to facilatate the conversion between DTOs and entities, using AutoMapper.
<br>
<br>
A custom middleware is added to enable Swagger UI to be served via APIM, and also enforce security validation to ensure that the APIM is not being bypassed to honor requests.
 
The Entity framework Core is used in this project to scaffold an existing database and any database operation was updated using migrations and the database is queried using LINQ.


## 4.	Infrastructure Implementation
The overall architecture is provided below to get a brief overview of the Implementation:<br>
<br>
![image](https://github.com/user-attachments/assets/42fc9e9b-4572-49d5-985b-aa5b77f8c6d0)

<br>
 
1)	The app is hosted in Azure App service which uses managed identities to validate authentication and authorization to retrieve the connection strings for the hosted SQL database.
2)	After authentication, it accesses the app configuration setting, which holds the key vault reference for the secret.
3)	Key Vault secret is accessed, as appropriate RBAC role is assigned to the managed identity of the app.
4)	The connection string is used to access the SQL database and the retrieve the appropriate data.
5)	If the requested data consists of image URIs, it is retrieved from the blob container of the azure storage account.
6)	Whenever an order is created, an azure function is triggered upon row insertion.
7)	Like the web application, the azure function also uses managed identities.
8)	Using which, it can retrieve the SAS key, stored in the Key Vault, to send messages to Azure Service bus Queue, 
9)	The changes are serialized into a JSON format and pushed into the queue.
10)	The pushed message can be consumed by any CRM/ERP application to do inventory management.
