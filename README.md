# Tickefy

![build](https://img.shields.io/badge/build-local-lightgrey)
![coverage](https://img.shields.io/badge/coverage-70%25-lightgrey)

A RESTful ticketing system API built with ASP.NET Core and a clean architecture layout (API, Application, Domain, Infrastructure).

## Quickstart

### Prerequisites

- .NET 8 SDK
- PostgreSQL

### Setup

1. Configure environment variables. You can copy `.env.example` to `.env` and fill in values.
2. Restore dependencies.
3. Build and run the API.

```powershell
copy .env.example .env

dotnet restore

dotnet build

dotnet run --project src\Tickefy.API
```

### Tests

```powershell
dotnet test
```

### Format and Lint

This project uses `dotnet format`.

```powershell
dotnet format --verify-no-changes
```

## Configuration

The API reads configuration from environment variables. Keep secrets out of source control.

| Name | Required | Description | Example |
| --- | --- | --- | --- |
| DB_HOST | Yes | PostgreSQL host | localhost |
| DB_PORT | Yes | PostgreSQL port | 5432 |
| DB_NAME | Yes | Database name | tickefy |
| DB_USER | Yes | Database user | postgres |
| DB_PASSWORD | Yes | Database password | your_password |
| JWT_KEY | Yes | JWT signing key | your_secret_key |
| JWT_ISSUER | Yes | JWT issuer | tickefy |
| JWT_AUDIENCE | Yes | JWT audience | tickefy_client |
| JWT_VALIDITY_MINS | No | Token validity in minutes | 30 |
| GEMINI_API_KEY | No | Gemini API key (AI classification) | your_api_key |

## Common Commands

```powershell
# Build
scripts\build.ps1

# Test
scripts\test.ps1

# Format check
scripts\format.ps1
```

## Project Structure

- src/ - application source code
- tests/ - unit tests
- docs/ - architecture decisions

## Architecture Overview

The API follows a clean architecture approach. Controllers in the API layer depend on application-level commands and queries via MediatR. Domain logic is isolated from infrastructure concerns, while data persistence is handled in the Infrastructure layer using EF Core with PostgreSQL.

Authentication uses JWT bearer tokens. Authorization is role-based for Admin, Manager, Agent, and Requester roles.

## Decisions

- ADR template: docs/adr/ADR-000-template.md
- Tech stack ADR: docs/adr/ADR-001-tech-stack.md

## Roadmap / Next Steps

- Add integration tests with a disposable database.
- Add CI pipeline for build, tests, and format checks.
- Add API versioning and OpenAPI documentation polish.

## Contributing

See CONTRIBUTING.md for development workflow and expectations.

## Security

See SECURITY.md to report vulnerabilities.

## Coverage Target

Target unit test coverage: 70%.

## What to Modify

- Add or update API endpoints in src/Tickefy.API controllers.
- Add business logic in src/Tickefy.Application handlers.
- Add domain entities and rules in src/Tickefy.Domain.
- Add persistence logic in src/Tickefy.Infrastructure.
