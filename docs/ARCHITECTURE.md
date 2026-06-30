# Tickefy — Architecture Overview

> **Stack:** .NET 8 · ASP.NET Core · PostgreSQL · Entity Framework Core · MediatR · FluentValidation · AutoMapper · Google Gemini AI · Serilog · xUnit

---

## Table of Contents

1. [High-Level Architecture](#1-high-level-architecture)
2. [Solution Layout](#2-solution-layout)
3. [Layer Responsibilities](#3-layer-responsibilities)
4. [Domain Model](#4-domain-model)
5. [Result Pattern](#5-result-pattern)
6. [Ticket State Machine](#6-ticket-state-machine)
7. [CQRS with MediatR](#7-cqrs-with-mediatr)
8. [Application Handlers](#8-application-handlers)
9. [AI Integration](#9-ai-integration)
10. [Authentication & Refresh Token Flow](#10-authentication--refresh-token-flow)
11. [Infrastructure Layer Detail](#11-infrastructure-layer-detail)
12. [API Layer Detail](#12-api-layer-detail)
13. [Request Lifecycle (End-to-End Flow)](#13-request-lifecycle-end-to-end-flow)
14. [Dependency Graph](#14-dependency-graph)
15. [Testing](#15-testing)
16. [Configuration & Environment Variables](#16-configuration--environment-variables)
17. [Key Design Decisions](#17-key-design-decisions)

---

## 1. High-Level Architecture

Tickefy is a **support-ticket management API** built with **Clean Architecture** (also called Onion Architecture). The dependency rule is strictly enforced: inner layers know nothing about outer layers.

```
┌─────────────────────────────────────────────────────────┐
│                      Tickefy.API                        │
│  Controllers · DTOs · Mapping · Error Handling · Auth   │
├─────────────────────────────────────────────────────────┤
│                  Tickefy.Application                    │
│  CQRS Handlers · Validators · Abstractions · Mapping    │
├─────────────────────────────────────────────────────────┤
│                  Tickefy.Infrastructure                  │
│  EF Core · Repositories · TokenService · AiService      │
├─────────────────────────────────────────────────────────┤
│                    Tickefy.Domain                       │
│  Entities · Aggregates · Result · Errors · Interfaces   │
└─────────────────────────────────────────────────────────┘
         ↑ only inner layers are referenced outward ↓
```

The **Domain** layer is the core and has **zero external dependencies**. Every other layer depends on it, never the other way around.

---

## 2. Solution Layout

```
Tickefy/
├── src/
│   ├── Tickefy.Domain/            # Core domain model
│   │   ├── ActivityLog/           # ActivityLog entity + repository interface
│   │   ├── Attachment/            # Attachment entity + repository interface
│   │   ├── Comment/               # Comment entity + repository interface
│   │   ├── Common/
│   │   │   ├── Action/            # TicketAction enum
│   │   │   ├── Category/          # Category enum
│   │   │   ├── EntityBase/        # Generic base class for all entities
│   │   │   ├── Errors/            # Error hierarchy (Error, NotFoundError, …)
│   │   │   ├── Event/             # EventType enum (for activity logs)
│   │   │   ├── Priority/          # Priority enum
│   │   │   ├── Results/           # IResult, Result, Result<T>
│   │   │   ├── Status/            # Ticket Status enum
│   │   │   └── UserRole/          # UserRoles enum
│   │   ├── Primitives/            # Strongly-typed ID base + concrete IDs
│   │   ├── RefreshToken/          # RefreshToken entity + repository interface
│   │   ├── Team/                  # Team entity + repository interface
│   │   ├── Ticket/                # Ticket aggregate root + repository interface
│   │   └── User/                  # User entity + repository interface
│   │
│   ├── Tickefy.Application/       # Use cases
│   │   ├── Abstractions/
│   │   │   ├── Data/              # IUnitOfWork
│   │   │   ├── Messaging/         # ICommand, IQuery, ICommandHandler, IQueryHandler
│   │   │   └── Services/          # IAiService, IAiResponseParser, IPasswordHasher, ITokenService
│   │   ├── AI/Dtos/               # AiResponse DTO
│   │   ├── ActivityLog/           # GetAll, GetByTicketId handlers
│   │   ├── Auth/                  # Login, Register, SetPassword, Logout, RefreshToken handlers
│   │   ├── Mapping/               # AutoMapper profiles for Application objects
│   │   ├── PipelineBehaviors/     # ValidationBehavior<TRequest, TResponse>
│   │   ├── Team/                  # Create, Delete, AddMember, RemoveMember, GetAll, GetById, GetMy
│   │   ├── Ticket/                # 16 use-case folders (Create, CreateDraft, Publish, Take, …)
│   │   └── User/                  # GetAll, GetById, Delete, SetRole, UpdateProfile
│   │
│   ├── Tickefy.Infrastructure/    # Persistence & external services
│   │   ├── Database/              # AppDbContext, UnitOfWork, AppDbContextFactory
│   │   ├── Migrations/            # EF Core code-first migrations
│   │   ├── Options/               # JwtSettings POCO
│   │   ├── Repositories/          # EF implementations of domain repository interfaces
│   │   └── Services/
│   │       ├── AI/                # AiService (Gemini) + AiResponseParser
│   │       ├── PasswordHasher.cs  # BCrypt implementation
│   │       └── TokenService.cs    # JWT + refresh token generation
│   │
│   └── Tickefy.API/               # Presentation layer
│       ├── ActivityLog/           # ActivityLogController + DTOs
│       ├── Auth/                  # AuthController + DTOs
│       ├── ErrorHandling/         # GlobalExceptionHandler, ErrorExtensions, ResultExtensions
│       ├── Mapping/               # API-level AutoMapper profiles
│       ├── Team/                  # TeamController + DTOs
│       ├── Ticket/                # TicketController + DTOs
│       ├── User/                  # UserController + DTOs
│       └── Program.cs             # DI composition root, middleware pipeline
│
├── tests/
│   ├── Tickefy.Domain.Tests/      # Domain unit tests (state machines, builders)
│   └── Tickefy.Application.Tests/ # Application handler tests
│
├── docs/adr/                      # Architecture Decision Records
├── scripts/                       # build.ps1, format.ps1, test.ps1
├── .env.example                   # Environment variable template
└── CHANGELOG.md
```

---

## 3. Layer Responsibilities

### 3.1 Domain Layer

**`Tickefy.Domain`** — the innermost ring. Has **no NuGet dependencies** besides the .NET base class library.

Responsibilities:
- Define **aggregate roots** and **entities** with private setters (encapsulation)
- Encode **business rules** directly on entities (e.g., `Ticket.Cancel()` checks `GetAvailableActions()`)
- Provide the **Result pattern** for communicating success/failure without exceptions
- Define **repository interfaces** (contracts only — no implementations)
- Define **service interfaces** (`IPasswordHasher`, `ITokenService`, etc.)
- Define **strongly-typed IDs** as records to prevent ID-type confusion

### 3.2 Application Layer

**`Tickefy.Application`** — orchestrates use cases. Depends only on Domain.

Responsibilities:
- Implement **CQRS use cases** as command and query handlers via MediatR
- Define **validation rules** using FluentValidation (one validator per command)
- Run **validation automatically** via the `ValidationBehavior` MediatR pipeline
- Define **service abstractions** (e.g., `IAiService`) consumed by handlers
- Host **AutoMapper profiles** for Application-level data transformations
- Contain no EF Core, HTTP, or infrastructure concerns

### 3.3 Infrastructure Layer

**`Tickefy.Infrastructure`** — implements interfaces declared in Domain and Application. Depends on both.

Responsibilities:
- Implement **EF Core repositories** for every aggregate
- Configure **entity mappings**, **foreign keys**, **indexes**, and **value conversions** (strongly-typed IDs)
- Run **code-first migrations** (managed via `AppDbContextFactory`)
- Implement **`ITokenService`** — JWT creation and random refresh token generation
- Implement **`IPasswordHasher`** — BCrypt hashing and verification
- Implement **`IAiService`** — Google Gemini API call and raw JSON extraction
- Implement **`IAiResponseParser`** — parse AI JSON response into domain enums
- Provide **`UnitOfWork`** wrapping `AppDbContext.SaveChangesAsync`

### 3.4 API Layer

**`Tickefy.API`** — the outer shell. Depends on Application and Infrastructure (only for DI wiring in `Program.cs`).

Responsibilities:
- Host **ASP.NET Core controllers** that translate HTTP → CQRS and Result → HTTP
- Define **request/response DTOs** and validate them via FluentValidation (delegated through MediatR)
- Define **extension methods** (`Request.ToCommand(...)`) for clean mapping from HTTP request to command/query
- Handle **global exceptions** via `GlobalExceptionHandler` (`IExceptionHandler`)
- Map **domain `Error`** values to RFC 7807 `ProblemDetails` via `ErrorExtensions`
- Configure **JWT Bearer authentication**, CORS, Swagger, Serilog
- Act as the **DI composition root** — all interface-to-implementation bindings are in `Program.cs`

---

## 4. Domain Model

### 4.1 Entities

All domain entities extend the generic base class:

```csharp
public class EntityBase<T> where T : StronglyTypedId<T>
{
    public T Id { get; protected set; }
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }

    protected void OnCreate() => Created = DateTime.UtcNow;
    protected void OnModify() => Modified = DateTime.UtcNow;
}
```

Every entity calls `OnCreate()` in its factory method and `OnModify()` when mutated (where relevant).

#### `Ticket` (Aggregate Root)

| Property | Type | Notes |
|---|---|---|
| `Id` | `TicketId` | Strongly-typed Guid |
| `Title` | `string` | |
| `Description` | `string` | |
| `RequesterId` | `UserId` | FK → User |
| `AssignedTeamId` | `TeamId?` | FK → Team, nullable |
| `AssignedAgentId` | `UserId?` | FK → User, nullable |
| `Category` | `Category?` | Set by AI on create |
| `Priority` | `Priority?` | Set by AI on create |
| `Status` | `Status` | Driven by state machine |
| `Deadline` | `DateTime` | |
| `Comments` | `List<Comment>` | Child collection |
| `Attachments` | `List<Attachment>` | Child collection |

All mutation methods (`Take`, `Complete`, `Cancel`, etc.) return `Result` — they never throw.

#### `User`

| Property | Type | Notes |
|---|---|---|
| `Id` | `UserId` | |
| `FirstName` | `string` | |
| `LastName` | `string` | |
| `Login` | `string` | Unique index in DB |
| `PasswordHash` | `string` | BCrypt hash |
| `Role` | `UserRoles` | Default: `Requester` |
| `TeamId` | `TeamId?` | FK → Team |

Created via `User.Create(...)`, defaults to `UserRoles.Requester`.

#### `Team`

| Property | Type | Notes |
|---|---|---|
| `Id` | `TeamId` | |
| `Name` | `string` | Unique index in DB |
| `Description` | `string?` | |
| `Category` | `Category` | Domain category for assignment routing |
| `ManagerId` | `UserId` | Leader of the team |
| `Members` | `List<User>` | Users in the team |

`AddMember` idempotently adds a user and calls `user.SetTeam(this)`.
`RemoveMember` returns `Result` — fails with `NotFoundError` if user is not a member.

#### `ActivityLog`

| Property | Type | Notes |
|---|---|---|
| `Id` | `ActivityLogId` | |
| `TicketId` | `TicketId` | FK → Ticket (cascade delete) |
| `UserId` | `UserId` | FK → User (restrict delete) |
| `EventType` | `EventType` | Enum: `RequestCreated`, etc. |
| `Description` | `string` | Human-readable entry |

Created by handlers after each significant ticket transition.

#### `RefreshToken`

| Property | Type | Notes |
|---|---|---|
| `Id` | `TokenId` | |
| `UserId` | `UserId` | FK → User |
| `Token` | `string` | Unique index, Base64 random |
| `ExpiresAt` | `DateTime` | 7-day TTL |

#### `Comment`

Belongs to a `Ticket`. Stores `Content`, `UserId`, and `TicketId`.

#### `Attachment`

Belongs to a `Ticket`. Stores `FilePath`, `FileName`, `SizeBytes`, `ContentType`.

---

### 4.2 Strongly-Typed IDs

All entity IDs use a custom record base to prevent passing the wrong ID type to a method:

```csharp
public abstract partial record StronglyTypedId<T> where T : StronglyTypedId<T>
{
    protected StronglyTypedId(Guid id) => Value = id;
    protected StronglyTypedId() => Value = Guid.NewGuid();   // generates new ID
    public Guid Value { get; }
}
```

Concrete IDs are minimal sealed records:

```csharp
public sealed record UserId(Guid Value) : StronglyTypedId<UserId>;
public sealed record TicketId(Guid Value) : StronglyTypedId<TicketId>;
// ... TeamId, ActivityLogId, CommentId, AttachmentId, TokenId
```

EF Core needs to know how to map these to `Guid` columns. A custom extension applies the conversion:

```csharp
modelBuilder.Entity<Ticket>().HasStronglyTypedIdConversion(a => a.Id);
```

This registers a `ValueConverter<TicketId, Guid>` so EF Core stores the raw GUID in the database.

---

### 4.3 Enumerations

| Enum | Values |
|---|---|
| `Status` | `Draft`, `Created`, `Assigned`, `InProgress`, `Completed`, `Accepted`, `Reopened`, `Canceled`, `Failed` |
| `TicketAction` | `Publish`, `Take`, `StartWork`, `Complete`, `Accept`, `Reopen`, `Cancel`, `Fail` |
| `Category` | `Finance`, `IT`, `Design`, `Marketing`, `HumanResources`, `Legal`, `AccessAndSecurity`, `Other` |
| `Priority` | `Critical`, `High`, `Medium`, `Low` |
| `UserRoles` | `Requester`, `Agent`, `Admin`, `Manager` |
| `EventType` | `RequestCreated` (and others for each ticket lifecycle event) |

---

### 4.4 Entity Relationships

```
User ──< Ticket (as Requester)
User ──< Ticket (as AssignedAgent, nullable)
Team ──< Ticket (as AssignedTeam, nullable)
Team >── User (Manager)
Team ──< User (Members, via User.TeamId FK)
Ticket ──< Comment
Ticket ──< Attachment
Ticket ──< ActivityLog
User ──< ActivityLog
User ──< RefreshToken
```

**Delete behaviors configured in `AppDbContext.OnModelCreating`:**

| Relationship | Delete Behavior |
|---|---|
| `ActivityLog → Ticket` | Cascade |
| `ActivityLog → User` | Restrict |
| `Comment → Ticket` | Cascade |
| `Comment → User` | Restrict |
| `Attachment → Ticket` | Cascade |
| `Ticket → Requester (User)` | Restrict |
| `Ticket → AssignedAgent` | Restrict |
| `Ticket → AssignedTeam` | Restrict |
| `Team → Manager (User)` | SetNull |
| `User → Team` | SetNull |

---

## 5. Result Pattern

Tickefy entirely avoids throwing exceptions for business logic. All domain methods and Application handlers communicate outcomes through the **Result pattern**.

### 5.1 Error Hierarchy

```
Error(string message)
├── NotFoundError        → HTTP 404
├── ForbiddenError       → HTTP 403
├── AlreadyExistsError   → HTTP 409
└── InvalidArgumentError → HTTP 400
```

Each derived type carries only the message string from the base `Error` class.

### 5.2 Result Types

**`Result`** — for void operations (no value on success):

```csharp
public class Result : IResult
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new Result();
    public static Result Failure(Error error) => new Result(error);
}
```

**`Result<T>`** — for operations that return a value:

```csharp
public class Result<T> : IResult<T>
{
    public T Value { get; }
    public Error Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public static Result<T> Success(T value) => new Result<T>(value);
    public static Result<T> Failure(Error error) => new Result<T>(error);

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure)
        => IsSuccess ? onSuccess(Value) : onFailure(Error);
}
```

Both types implement `IResult` / `IResult<T>` interfaces defined in Domain.

### 5.3 Error-to-HTTP Mapping

In the API layer, two extension methods handle result dispatch:

**`ResultExtensions.Match`** — for `Result` (void success):
```csharp
// Usage in controller:
return result.Match(Ok(), this.ToActionResult);
```

**`Result<T>.Match`** — for `Result<T>` (value success):
```csharp
return result.Match(
    onSuccess: value => Ok(_mapper.Map<TicketResponse>(value)),
    onFailure: this.ToActionResult);
```

**`ErrorExtensions.ToActionResult`** — maps `Error` → `ProblemDetails` → `IActionResult`:

```csharp
private static ProblemDetails ToProblemDetails(this Error error) => error switch
{
    NotFoundError        => { Status = 404, Title = "Not Found",      Detail = error.Message },
    ForbiddenError       => { Status = 403, Title = "Forbidden",      Detail = error.Message },
    AlreadyExistsError   => { Status = 409, Title = "Already exists", Detail = error.Message },
    InvalidArgumentError => { Status = 400, Title = "Bad request",    Detail = error.Message },
    _                    => throw new NotImplementedException()
};
```

---

## 6. Ticket State Machine

The `Ticket` aggregate encodes its own valid state transitions. `GetAvailableActions()` returns the allowed `TicketAction` values for the current status. Every mutation method checks this before acting:

```
                    ┌──────────────────┐
               ╔════╡      Draft       ╞═══╗
               ║    └──────────────────┘   ║
               ║      [Publish action]      ║
               ▼                           ▼
    ┌──────────────────┐        ┌──────────────────┐
    │     Created      │─Cancel─▶    Canceled       │
    └──────────────────┘        └──────────────────┘
               │ Take
               ▼
    ┌──────────────────┐
    │     Assigned     │─Cancel─▶ Canceled
    └──────────────────┘
               │ StartWork
               ▼
    ┌──────────────────┐
    │   InProgress     │─Cancel─▶ Canceled
    └──────────────────┘─Fail───▶ Failed
               │ Complete
               ▼
    ┌──────────────────┐
    │    Completed     │─Reopen─▶ Reopened ─StartWork─▶ InProgress (cycle)
    └──────────────────┘
               │ Accept
               ▼
    ┌──────────────────┐
    │    Accepted      │  (terminal)
    └──────────────────┘
```

**State → Available Actions (exact mapping from `Ticket.GetAvailableActions()`):**

| Status | Available Actions |
|---|---|
| `Draft` | `Publish` |
| `Created` | `Take`, `Cancel` |
| `Assigned` | `Cancel`, `StartWork` |
| `InProgress` | `Complete`, `Cancel`, `Fail` |
| `Completed` | `Accept`, `Reopen` |
| `Reopened` | `StartWork` |
| `Accepted`, `Canceled`, `Failed` | *(none — terminal)* |

If a handler calls e.g. `ticket.Complete()` when the ticket is in `Draft` status, the method returns `Result.Failure(new ForbiddenError("Invalid action"))` — no exception, no branching outside the domain.

---

## 7. CQRS with MediatR

### 7.1 Messaging Abstractions

Four marker interfaces in `Tickefy.Application.Abstractions.Messaging`:

```csharp
public interface ICommand : IRequest { }
public interface ICommand<out TResponse> : IRequest<TResponse> { }
public interface IQuery<out TResponse> : IRequest<TResponse> { }

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand { }

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse> { }

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse> { }
```

These wrappers make intent explicit (command vs. query) and allow future pipeline differentiation per message type.

### 7.2 Command / Query Pattern

Every use case is a **self-contained folder** with three files:

```
Ticket/Create/
├── CreateTicketCommand.cs          # ICommand<Result> — the input
├── CreateTicketCommandHandler.cs   # ICommandHandler<…> — the logic
└── CreateTicketCommandValidator.cs # AbstractValidator<CreateTicketCommand>
```

Commands carry only the data needed. They are constructed in the API layer via `Request.ToCommand(userId)` extension methods and dispatched via `_mediator.Send(command)`.

Queries follow the same layout but are read-only (no side effects).

### 7.3 Validation Pipeline Behavior

Every MediatR request passes through `ValidationBehavior<TRequest, TResponse>` **before** reaching the handler:

```
HTTP Request
    ↓
[Controller]
    ↓  mediator.Send(command)
[MediatR Pipeline]
    ↓
[ValidationBehavior]  ← FluentValidation validators discovered by DI
    │  if any failures → throw ValidationException
    ↓
[CommandHandler / QueryHandler]
    ↓
Result<T>
```

**Registration in `Program.cs`:**
```csharp
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<LoginUserCommandHandler>();
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssemblyContaining<AddMemberCommandValidator>();
```

Validators are discovered automatically from the Application assembly. If a command has no registered validator, `ValidationBehavior` skips validation silently.

---

## 8. Application Handlers

### 8.1 Auth Handlers

| Handler | Command/Query | Returns | Description |
|---|---|---|---|
| `RegisterUserCommandHandler` | `RegisterUserCommand` | `Result` | Hashes password, saves new `User`, persists via UoW |
| `LoginUserCommandHandler` | `LoginUserCommand` | `Result<LoginResult>` | Validates credentials, generates JWT + refresh token, saves `RefreshToken` |
| `SetPasswordCommandHandler` | `SetPasswordCommand` | `Result` | Hashes new password, updates user |
| `RefreshTokenCommandHandler` | `RefreshTokenCommand` | `Result<LoginResult>` | Validates refresh token expiry, issues new JWT + refresh token, deletes old |
| `LogoutCommandHandler` | `LogoutCommand` | `Result` | Deletes refresh token by value, saves |

### 8.2 Ticket Handlers

| Handler | Returns | Key Logic |
|---|---|---|
| `CreateTicketCommandHandler` | `Result` | Creates `Ticket`, calls AI for Category/Priority (graceful fallback), adds `ActivityLog`, saves |
| `CreateDraftTicketCommandHandler` | `Result` | Same as create but status = `Draft`, no AI call |
| `PublishTicketCommandHandler` | `Result` | Loads ticket, checks ownership, calls `ticket.Publish(…)`, saves |
| `TakeTicketCommandHandler` | `Result` | Loads ticket + agent's team (by category match), calls `ticket.Take(agentId, teamId)`, saves |
| `StartWorkTicketCommandHandler` | `Result` | Loads ticket, checks agent role, calls `ticket.StartWork()`, saves |
| `CompleteTicketCommandHandler` | `Result` | Loads ticket, checks agent role, calls `ticket.Complete()`, saves |
| `AcceptTicketCommandHandler` | `Result` | Loads ticket, checks requester role, calls `ticket.Accept()`, saves |
| `ReopenTicketCommandHandler` | `Result` | Loads ticket, checks requester role, calls `ticket.Reopen()`, saves |
| `CancelTicketCommandHandler` | `Result` | Loads ticket, checks role, calls `ticket.Cancel()`, saves |
| `FailTicketCommandHandler` | `Result` | Loads ticket (Admin only), calls `ticket.Fail()`, saves |
| `PostCommentCommandHandler` | `Result` | Loads ticket, creates `Comment`, adds to ticket, saves |
| `GetMyTicketsQueryHandler` | `Result<List<Ticket>>` | Fetches tickets by userId |
| `GetAllTicketsQueryHandler` | `Result<List<Ticket>>` | Admin-only: returns all tickets |
| `GetQueueTicketsQueryHandler` | `Result<List<Ticket>>` | Agent: `Created` tickets matching agent's team category |
| `GetTicketByIdQueryHandler` | `Result<TicketDetailsResult>` | Loads ticket, checks access, builds result with available actions |

### 8.3 Team Handlers

| Handler | Returns | Key Logic |
|---|---|---|
| `CreateTeamCommandHandler` | `Result` | Creates `Team`, sets manager, saves |
| `AddMemberCommandHandler` | `Result<Unit>` | Resolves user by login, calls `team.AddMember(user)`, saves |
| `RemoveMemberCommandHandler` | `Result<Unit>` | Calls `team.RemoveMember(user)`, saves |
| `DeleteTeamCommandHandler` | `Result<Unit>` | Loads team (leader check), deletes, saves |
| `GetAllTeamsQueryHandler` | `Result<List<Team>>` | Returns all teams |
| `GetTeamByIdQueryHandler` | `Result<Team>` | Returns single team with members |
| `GetTeamByUserIdQueryHandler` | `Result<List<Team>>` | Returns teams where user is manager or member |

### 8.4 User Handlers

| Handler | Returns | Key Logic |
|---|---|---|
| `GetAllUsersQueryHandler` | `Result<List<User>>` | Admin: all users |
| `GetUserByIdQueryHandler` | `Result<User>` | Admin: single user |
| `DeleteUserQueryHandler` | `Result<Unit>` | Admin: delete user |
| `SetUserRoleCommandHandler` | `Result<Unit>` | Admin: update role |
| `UpdateProfileCommandHandler` | `Result<Unit>` | Authenticated user: update name |

### 8.5 Activity Log Handlers

| Handler | Returns | Key Logic |
|---|---|---|
| `GetAllLogsQueryHandler` | `List<ActivityLog>` | Paged list ordered by creation date descending |
| `GetLogsByTicketIdQueryHandler` | `List<ActivityLog>` | All logs for a given ticket |

---

## 9. AI Integration

When a ticket is created (`POST /api/v1/tickets`), the handler classifies the ticket automatically via Gemini:

```
CreateTicketCommandHandler
    │
    ├─► IAiService.AnalyzeTicketAsync(title, description, deadline)
    │       │
    │       └─► Google.GenAI.Client → Gemini 2.0 Flash
    │               prompt: "classify ticket → return JSON {Category, Priority}"
    │               returns: raw JSON string
    │
    ├─► IAiResponseParser.ParseCategory(AiResponse)
    ├─► IAiResponseParser.ParsePriority(AiResponse)
    │
    ├─► ticket.SetCategory(category)
    ├─► ticket.SetPriority(priority)
    │
    └─► [on exception] fallback: Category.Other, Priority.Medium
```

**`AiService`** (`Tickefy.Infrastructure.Services.AI`):
- Accepts `Google.GenAI.Client` (singleton, API key from `GEMINI_API_KEY` env var)
- Builds a structured prompt enforcing strict JSON-only output (`{Category, Priority}`)
- Calls `client.Models.GenerateContentAsync(model: "gemini-2.0-flash", …)`
- Concatenates all text parts from the response candidates
- Deserializes with `PropertyNameCaseInsensitive = true`
- Logs raw JSON at `Information` level, parsed values at `Warning` level

**`AiResponseParser`** (`Tickefy.Infrastructure.Services.AI`):
- Uses `Enum.TryParse` (case-insensitive) to convert string → `Category` / `Priority`
- Falls back to `Category.Other` / `Priority.Medium` if parsing fails

AI failure **never prevents ticket creation** — defaults are always used on any exception.

---

## 10. Authentication & Refresh Token Flow

### JWT Access Token

- **Algorithm:** HMAC-SHA512
- **Claims:** `NameIdentifier` (UserId Guid), `Name` (Login), `Role`
- **Validity:** `JWT_VALIDITY_MINS` env var (default: 30 min)
- **Delivery:** `Authorization: Bearer <token>` header

### Refresh Token

- **Format:** 64-byte cryptographically random data, Base64-encoded
- **Storage:** `RefreshTokens` table with unique index on `Token`
- **Delivery:** `HttpOnly; SameSite=Strict` cookie named `refresh_token`
- **TTL:** 7 days

### Login Flow

```
POST /api/v1/auth/login
    │
    ├─ GetByLoginAsync → NotFoundError if missing
    ├─ BCrypt.Verify   → InvalidArgumentError if wrong password
    ├─ TokenService.GetToken           → JWT string
    ├─ TokenService.GenerateRefreshToken → Base64 string
    ├─ RefreshToken.Create(userId, expiresAt+7d, token)
    ├─ refreshTokenRepository.Add(entity)
    └─ unitOfWork.SaveChangesAsync()
    │
    AuthController
        ├─ Appends HttpOnly refresh_token cookie
        └─ Returns LoginResponse (JWT, user info)
```

### Refresh Flow

```
POST /api/v1/auth/refresh
    │
    ├─ Read refresh_token cookie → 401 if missing
    ├─ refreshTokenRepository.GetToken → NotFoundError
    ├─ Check ExpiresAt > UtcNow        → ForbiddenError if expired
    ├─ TokenService.GetToken           → new JWT
    ├─ TokenService.GenerateRefreshToken → new Base64
    ├─ refreshTokenRepository.DeleteByToken(old)
    ├─ refreshTokenRepository.Add(new entity)
    └─ unitOfWork.SaveChangesAsync()
    │
    AuthController → new cookie + new LoginResponse
```

### Logout Flow

```
POST /api/v1/auth/logout
    │
    ├─ Read refresh_token cookie → 401 if missing
    ├─ refreshTokenRepository.GetToken → NotFoundError
    ├─ refreshTokenRepository.Delete(entity)
    └─ unitOfWork.SaveChangesAsync()
    │
    AuthController → clears cookie (set to empty string)
```

---

## 11. Infrastructure Layer Detail

### 11.1 EF Core Configuration

`AppDbContext` is registered using `AddDbContextFactory<AppDbContext>` (allowing short-lived context creation from factories). Standard constructor injection is used in repositories.

`OnModelCreating` performs four steps for each entity:

1. **Ignores strongly-typed ID records** to prevent EF treating them as shadow entities
2. **Sets primary key** on the `Id` property
3. **Applies `HasStronglyTypedIdConversion`** — registers a `ValueConverter<TId, Guid>` so the DB stores raw GUIDs
4. **Configures relationships, unique indexes, max lengths, and delete behaviors**

**Notable DB constraints:**
- `User.Login` — unique index
- `Team.Name` — unique index
- `RefreshToken.Token` — unique index
- `Attachment.FilePath` — max 2048 chars
- `Comment.Content` — max 4000 chars

### 11.2 Repositories

All repositories follow the same pattern: `AppDbContext` injected via constructor, implementing the domain's repository interface via LINQ + EF Core.

```
ITicketRepository          ←  EFTicketRepository
IUserRepository            ←  EFUserRepository
ITeamRepository            ←  EFTeamRepository
IActivityLogRepository     ←  EFLogRepository
IRefreshTokenRepository    ←  EFRefreshTokenRepository
```

Repositories **never call `SaveChanges`**. All writes are committed only via `IUnitOfWork.SaveChangesAsync()` in the handler — enabling atomic batched commits.

Read queries use `.AsNoTracking()` where the entity is not being mutated. `Include()` is used explicitly per handler requirement to avoid over-fetching.

### 11.3 Unit of Work

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public UnitOfWork(AppDbContext context) => _context = context;

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
```

Handlers accumulate multiple repository operations and commit everything in one `SaveChangesAsync`, ensuring atomicity.

### 11.4 Services

| Service | Implementation | Key Details |
|---|---|---|
| `ITokenService` | `TokenService` | JWT via `System.IdentityModel.Tokens.Jwt`; refresh via `RandomNumberGenerator.GetBytes(64)` |
| `IPasswordHasher` | `PasswordHasher` | `BCrypt.Net.BCrypt.HashPassword` / `Verify` |
| `IAiService` | `AiService` | `Google.GenAI.Client` singleton; `gemini-2.0-flash` model |
| `IAiResponseParser` | `AiResponseParser` | `Enum.TryParse` with case-insensitive fallback |

---

## 12. API Layer Detail

### 12.1 Controllers

| Controller | Base Route | Primary Auth |
|---|---|---|
| `AuthController` | `api/v1/auth` | Mixed (register/login anonymous) |
| `TicketController` | `api/v1/tickets` | `[Authorize]` per endpoint + role checks |
| `TeamController` | `api/v1/teams` | `[Authorize(Roles = "…")]` per endpoint |
| `UserController` | `api/v1/users` | Mostly `[Authorize(Roles = "Admin")]` |
| `ActivityLogController` | `api/v1/logs` | `[Authorize(Roles = "Admin")]` |

The pattern for every action method:
1. Extract caller `UserId` from `User.FindFirst(ClaimTypes.NameIdentifier)`
2. `request.ToCommand(userId)` → typed command/query object
3. `await _mediator.Send(command)` → `Result<T>`
4. `.Match(onSuccess: …, onFailure: this.ToActionResult)`

### 12.2 Request / Response DTOs

Each controller folder has `Requests/` and `Responses/` sub-folders.

Request objects define an extension method:
```csharp
public class CreateTicketRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Deadline { get; set; }

    public CreateTicketCommand ToCommand(UserId userId) =>
        new CreateTicketCommand { Title = Title, Description = Description,
                                  Deadline = Deadline, UserId = userId };
}
```

This keeps controllers thin — they never construct commands directly.

### 12.3 AutoMapper Profiles

Two assemblies are scanned at startup:

| Assembly | Profiles |
|---|---|
| `Tickefy.Application` | `ActivityLogProfile`, `EnumProfile`, `StronglyTypedIdProfile`, `TeamProfile`, `TicketProfile`, `UserProfile` |
| `Tickefy.API` | `LoginMappingProfile` (auth response mappings) |

Key profiles:

**`StronglyTypedIdProfile`** — maps all strongly-typed IDs to `Guid`:
```csharp
CreateMap<TicketId, Guid>().ConvertUsing(src => src.Value);
```

**`EnumProfile`** — maps domain enums to string:
```csharp
CreateMap<Status, string>().ConvertUsing(src => src.ToString());
```

`AllowNullCollections = true` is configured globally so unmapped navigation properties don't throw.

### 12.4 Error Handling

**Tier 1 — Business Result Errors** (no exceptions):
`ErrorExtensions.ToActionResult` maps `Error` → `ProblemDetails` directly in the controller action.

**Tier 2 — Unhandled Exceptions** (FluentValidation, unexpected failures):
`GlobalExceptionHandler` (`IExceptionHandler`) catches everything else:
```csharp
public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
{
    var problemDetails = _mapper.Map(ex);     // ExceptionProblemDetailsMapper
    ctx.Response.StatusCode = problemDetails.Status ?? 500;
    return await _problemDetailsService.TryWriteAsync(…);
}
```

All `ProblemDetails` responses include a `traceId` extension from the current `Activity`.

### 12.5 CORS & Security

CORS policy `"AllowFrontend"`:
- Origins: `localhost:4200` + two LAN addresses
- Methods: GET, POST, PUT, DELETE, OPTIONS, PATCH
- Headers: `Content-Type`, `Authorization`
- **`AllowCredentials()`** — required for the `HttpOnly` cookie refresh token

JWT Bearer validation:
- Validates issuer, audience, lifetime, and signing key
- `RequireHttpsMetadata = false` (dev convenience)

### 12.6 Swagger / OpenAPI

- Enabled in **Development** only
- JWT Bearer input field registered via `AddSecurityDefinition("Bearer", …)`
- `EnableAnnotations()` reads `[SwaggerOperation]` attributes
- Security requirement applied globally

---

## 13. Request Lifecycle (End-to-End Flow)

### `POST /api/v1/tickets` — Create Ticket

```
Client
  │  POST /api/v1/tickets
  │  Authorization: Bearer <jwt>
  │  Body: { title, description, deadline }
  ▼
[Middleware Pipeline]
  1. ExceptionHandler (wraps everything)
  2. HTTPS Redirection
  3. CORS
  4. UseAuthentication → validates JWT, populates User.Claims
  5. UseAuthorization  → checks [Authorize]
  ▼
[TicketController.CreateAsync]
  1. Extract userId from ClaimTypes.NameIdentifier
  2. request.ToCommand(userId) → CreateTicketCommand
  3. await _mediator.Send(command)
  ▼
[MediatR Pipeline → ValidationBehavior]
  → CreateTicketCommandValidator.Validate(command)
  → throw ValidationException if invalid → GlobalExceptionHandler → 400
  ▼
[CreateTicketCommandHandler.Handle]
  1. Ticket.Create(title, description, userId, deadline)
  2. await _aiService.AnalyzeTicketAsync(…)           [Gemini API]
  3. ticket.SetCategory / SetPriority                 [or fallback]
  4. _ticketRepository.Add(ticket)                    [EF stages INSERT]
  5. ActivityLog.Create(ticketId, userId, Created)
  6. _logRepository.Add(log)                          [EF stages INSERT]
  7. await _unitOfWork.SaveChangesAsync()             [COMMIT transaction]
  8. return Result.Success()
  ▼
[TicketController]
  → result.Match(Created(), this.ToActionResult)
  ▼
HTTP 201 Created
```

### `PUT /api/v1/tickets/{id}/complete` — Complete Ticket

```
Client → JWT Auth → TicketController.CompleteTicketAsync
  ▼
[CompleteTicketCommandHandler]
  1. _ticketRepository.GetByIdAsync(id, ct) → NotFoundError if null
  2. Caller role check                       → ForbiddenError if not Agent/Admin
  3. ticket.Complete()
       ├─ GetAvailableActions() → [Complete, Cancel, Fail] (InProgress)
       ├─ Contains(Complete) = true
       ├─ Status = Completed
       └─ return Result.Success()
  4. _unitOfWork.SaveChangesAsync()
  5. return Result.Success()
  ▼
Controller → result.Match(Ok(), this.ToActionResult) → HTTP 200
```

---

## 14. Dependency Graph

```
Tickefy.Domain
  (no external dependencies)

Tickefy.Application
  └── Tickefy.Domain
  └── MediatR · FluentValidation · AutoMapper

Tickefy.Infrastructure
  └── Tickefy.Domain
  └── Tickefy.Application
  └── Microsoft.EntityFrameworkCore.Npgsql
  └── Google.GenAI · BCrypt.Net-Next · Microsoft.IdentityModel.Tokens

Tickefy.API
  └── Tickefy.Application  (all use-case interfaces)
  └── Tickefy.Infrastructure  (DI wiring only in Program.cs)
  └── Swashbuckle · Serilog · DotNetEnv · AutoMapper · MediatR
```

**Key external packages:**

| Package | Layer | Purpose |
|---|---|---|
| `MediatR` | Application / API | CQRS dispatch, pipeline behaviors |
| `FluentValidation` | Application | Command validation |
| `AutoMapper` | Application / API | Object-to-object mapping |
| `Microsoft.EntityFrameworkCore.Npgsql` | Infrastructure | PostgreSQL ORM |
| `Google.GenAI` | Infrastructure | Gemini AI API client |
| `BCrypt.Net-Next` | Infrastructure | Password hashing |
| `Microsoft.IdentityModel.Tokens` | Infrastructure / API | JWT creation and validation |
| `Serilog` | API | Structured console logging with enrichers |
| `Swashbuckle.AspNetCore` | API | Swagger / OpenAPI generation |
| `DotNetEnv` | API | Load `.env` file in development |

---

## 15. Testing

### `Tickefy.Domain.Tests`

Pure domain unit tests with **zero infrastructure dependencies**.

```
tests/Tickefy.Domain.Tests/
└── Ticket/
    ├── Builders/TicketBuilder.cs              # Fluent builder for test setup
    ├── TicketCreationTests.cs                 # Factory method tests
    ├── TicketStatusPositiveTransitions.cs     # Valid state transitions → IsSuccess
    └── TicketStatusNegativeTransitions.cs     # Invalid transitions → IsFailure
```

**`TicketBuilder`** provides a fluent API to create `Ticket` test fixtures in any state:
```csharp
var ticket = new TicketBuilder().WithStatus(Status.InProgress).Build();
```

**Positive tests:** assert `result.IsSuccess == true` after valid transitions.
**Negative tests:** assert `result.IsFailure == true` and verify `result.Error.Message` after invalid transitions (e.g., `Complete` on a `Draft` ticket).

### `Tickefy.Application.Tests`

Handler-level tests for the Application layer with mocked repositories.

```
tests/Tickefy.Application.Tests/
└── Tickets/
    ├── GetTicketByIdQueryHandler.cs        # Handler tests
    └── TicketActions/
        ├── CanExecutePositiveTests.cs      # ActionHelper.CanExecute true cases
        ├── CanExecuteNegativeTests.cs      # ActionHelper.CanExecute false cases
        └── RequireReasonTests.cs           # Validation for reason-required fields
```

---

## 16. Configuration & Environment Variables

All secrets are loaded from environment variables. In development, a `.env` file at the repo root is loaded automatically via `DotNetEnv.Env.Load("../../.env")`.

| Variable | Required | Description |
|---|---|---|
| `DB_HOST` | ✅ | PostgreSQL host |
| `DB_PORT` | ✅ | PostgreSQL port (typically `5432`) |
| `DB_NAME` | ✅ | Database name |
| `DB_USER` | ✅ | Database user |
| `DB_PASSWORD` | ✅ | Database password |
| `JWT_KEY` | ✅ | Secret key for HMAC-SHA512 JWT signing |
| `JWT_ISSUER` | ✅ | JWT issuer claim |
| `JWT_AUDIENCE` | ✅ | JWT audience claim |
| `JWT_VALIDITY_MINS` | ❌ | Access token validity (default: `30`) |
| `GEMINI_API_KEY` | ✅ | Google Gemini API key for AI classification |

A `.env.example` file at the repository root documents all variables with placeholder values.

---

## 17. Key Design Decisions

### Clean Architecture with strict dependency rule
Each layer only depends on the layer directly beneath it. Domain has zero external dependencies. Infrastructure wires everything together, but Application and Domain layers are independently testable.

### Result pattern instead of exceptions for business logic
Business failures are modelled as typed values, not exceptions. This makes control flow explicit and eliminates try/catch in handlers. Only genuinely unexpected failures (DB outage, AI crash) bubble up as exceptions.

### Strongly-typed IDs as records
Using `record TicketId(Guid Value)` prevents passing a `UserId` where a `TicketId` is expected — the compiler catches the mistake. EF Core handles the `Guid` ↔ `StronglyTypedId` conversion via custom value converters.

### `GetAvailableActions()` on the aggregate root
Rather than having permission logic scattered across handlers, `Ticket` is the single source of truth for what actions are valid at each status. Handlers simply call `ticket.Complete()` and let the domain return `ForbiddenError` if the state doesn't allow it.

### Validation via MediatR pipeline behavior
`ValidationBehavior<TRequest, TResponse>` runs FluentValidation before any handler executes. Validators run automatically without handlers needing to call them. A `ValidationException` on failure is caught by `GlobalExceptionHandler`, producing a `400 Bad Request`.

### AI graceful degradation
If the Gemini API is unavailable or returns malformed JSON, `CreateTicketCommandHandler` catches the exception, logs it, and continues with `Category.Other` / `Priority.Medium`. Ticket creation never fails due to AI unavailability.

### Refresh token as HttpOnly cookie
The refresh token is delivered via a `HttpOnly; SameSite=Strict` cookie, inaccessible to JavaScript. The frontend only stores the short-lived JWT in memory. This protects against XSS-based token theft while enabling seamless session renewal.

### Unit of Work for atomic commits
No repository calls `SaveChanges` directly. Handlers accumulate operations (add ticket, add log) and commit in a single `SaveChangesAsync()` call — all writes succeed or all fail atomically.

### `IDbContextFactory` registration
`AddDbContextFactory<AppDbContext>` is used instead of `AddDbContext` to enable short-lived context creation from factories — useful for future background services or scoped context overrides, without affecting standard constructor injection in repositories.
