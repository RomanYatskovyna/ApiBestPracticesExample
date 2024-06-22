# ApiBestPracticesExample

## Description

This project aims to provide a robust starting template for building REST APIs using the latest version of .NET. It incorporates best practices in software development to ensure scalability, maintainability, and performance. This template serves as a foundational framework that developers can build upon, accelerating the development process and ensuring a high standard of code quality.

## Key Features

1. **Clean Architecture**:
   - Adopts a clean architecture approach to separate concerns, ensuring that the system is modular, testable, and maintainable.
2. **FastEndpoints**:
   - Uses FastEndpoints to streamline the creation and management of API endpoints, improving performance and simplifying endpoint configuration with its extensive set of tools.

3. **Serilog Logging**:
   - Integrates Serilog for structured logging, enabling comprehensive and configurable logging across the application for easier debugging and monitoring.

4. **Entity Framework Core**:
   - Uses Entity Framework Core for database operations, providing a modern ORM with support for LINQ queries, change tracking, and migrations.

5. **OpenAPI Best Practices**:
   - Implements OpenAPI (Swagger) best practices for API documentation, making it easier to visualize and interact with the API endpoints. This ensures clear, consistent, and comprehensive API documentation.

6. **API Versioning Support**:
   - Supports API versioning to manage changes over time and ensure backward compatibility, allowing multiple versions of the API to coexist.

7. **Caching with Redis and InMemory**:
   - Implements caching using a common interface for both Redis and InMemory caches to enhance performance and scalability. This allows for flexible caching strategies depending on the environment and use case.

8. **Integration Testing with Testcontainers and Respawn**:
   - Facilitates integration testing using Testcontainers to create and manage containerized dependencies, ensuring tests run in a consistent environment.

## Installation

1. Clone the repository.
```shell
git clone https://github.com/RomanYatskovyna/ApiBestPracticesExample.git
```
2. Navigate to the project directory.
3. Add variables into **`Secrets.json`**.
4. Review **`appsettings`** variables.
5. Start local dependencies if required.

## Testing

Ensure Docker is running on the machine. Tests will automatically connect to it, create all the required containers, and run tests against them. Containers are shared between tests, and databases use Respawn to clean data between tests.
## Database


### Migrations

To create a migration, use:

```shell
Add-migration MigrationName
```

**All migrations are automatically applied during project startup.**

### Changes Introduction

To introduce new changes into the database:

1. **Create a temporary database and migrate it.**
2. **Introduce changes and scaffold them back to the project using the EF Core Power Tools extension.**
3. **Create a migration and verify that everything was created successfully.**
4. **Ensure all relevant data remains persistent in the database after applying the migration before using it on the production database.**

This approach, though slightly complicated, allows you to use UI database tools to visualize tables and write native SQL code that will be reverse-engineered into C#.

## License

This repository is licensed with the MIT license.