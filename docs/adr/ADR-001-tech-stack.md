# ADR-001: Adopt ASP.NET Core + Clean Architecture

## Status

Accepted

## Context

We need a maintainable, testable backend for a ticketing system with clear separation of concerns and long-term scalability.

## Decision

- Use .NET 8 and ASP.NET Core for the API.
- Apply a clean architecture layout: API, Application, Domain, Infrastructure.
- Use MediatR for command/query handling.
- Use EF Core with PostgreSQL for persistence.
- Use JWT for authentication and role-based authorization.
- Use Serilog for structured logging.
- Use xUnit for unit testing.

## Consequences

- The codebase is modular and testable, but adds upfront structure and conventions.
- Infrastructure changes are isolated from domain logic.
- Additional setup is required for local environment variables and PostgreSQL.

## Date

2026-05-24
