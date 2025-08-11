# Commex Shared Components

This repository contains the shared components for the Commex communication management system, including DTOs (Data Transfer Objects) and unit tests.

## Repository Structure

- **TSG-Commex-Shared/**: Contains shared DTOs and data contracts used across the Commex ecosystem
- **Commex-Unit-Tests/**: Contains unit tests for shared components and integration tests
- **database-schema.dbml**: Database schema definition
- **test-event.json**: Sample test event data

## Projects

### TSG-Commex-Shared
A .NET 8 class library containing:
- DTOs for requests and responses
- Shared data models
- Communication types and status definitions

### Commex-Unit-Tests  
A .NET 8 test project containing:
- Unit tests for shared DTOs
- Integration tests
- Test utilities and mocks

## Related Repositories

This is part of a multi-repository architecture:
- **Frontend Repository**: [TSG-Commex-FE](https://github.com/York-Solutions-B2E/TSG-Commex-FE) - Contains the Blazor Server frontend application
- **Backend Repository**: [TSG-Commex-BE](https://github.com/York-Solutions-B2E/TSG-Commex-BE) - Contains the API backend services
- **Shared Repository** (this repo): Contains shared DTOs and tests

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or Visual Studio Code

### Building the Solution
```bash
dotnet restore
dotnet build
```

### Running Tests
```bash
dotnet test
```

## Architecture Notes

The shared DTOs in this repository are designed to be consumed by both the frontend and backend applications as NuGet packages or project references, ensuring consistent data contracts across the entire system.

## Dependencies

The unit tests include:
- NUnit for testing framework
- Moq for mocking
- .NET Test SDK for test execution

## Contributing

When making changes to shared DTOs:
1. Ensure backward compatibility
2. Update corresponding tests
3. Version appropriately for breaking changes
4. Coordinate with frontend/backend teams for deployment