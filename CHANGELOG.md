# Changelog

Format: update - date - name (#number)

## update - 2026-06-28 - XML documentation completion (fix/docs-and-naming)
**Goal:** Ensure every documented interface method and API endpoint has a complete set of XML tags.
**Description:** Added missing `<returns>` tags to all querying methods across `ITicketRepository`, `IUserRepository`, `ITeamRepository`, `IActivityLogRepository`, `IRefreshTokenRepository`, `IUnitOfWork`, `IAiService`, `IAiResponseParser`, `IPasswordHasher`, and `ITokenService`. Added interface-level `<summary>` to `IRefreshTokenRepository`. Added XML documentation (`<summary>` and `<returns>`) to the previously undocumented `Refresh` and `Logout` endpoints in `AuthController`. Extended `<returns>` tags to all endpoint methods across `TicketController`, `AuthController`, `UserController`, `TeamController`, and `ActivityLogController`. Improved summary wording on `ITokenService.GenerateRefreshToken` and `IRefreshTokenRepository.DeleteByToken`/`Delete`.

## update - 2026-06-28 - Refresh token support (#38)
**Goal:** Add stateful refresh token flow so clients can extend sessions without re-authenticating.
**Description:** Introduced `RefreshToken` domain entity and `IRefreshTokenRepository` with EF Core implementation and three migrations. Added `RefreshTokenCommand`/`RefreshTokenCommandHandler` and `LogoutCommand`/`LogoutCommandHandler` in the Application layer. Updated `LoginUserCommandHandler` to persist a refresh token on login. Exposed `POST /auth/refresh` and `POST /auth/logout` endpoints in `AuthController`, with the refresh token delivered via an `HttpOnly` cookie. Extended `ITokenService` with `GenerateRefreshToken` and wired the new repository in `Program.cs`.

## update - 2026-06-21 - Documentation improvement (#37)
**Goal:** Improve project documentation with endpoint docs and interface summaries.
**Description:** Added XML `<summary>` comments to all controller endpoints (`ActivityLogController`, `AuthController`, `TeamController`, `TicketController`, `UserController`) and to all interface methods across `IUnitOfWork`, `ICommand`, `ICommandHandler`, `IQuery`, `IQueryHandler`, `IAiResponseParser`, `IAiService`, `IPasswordHasher`, `ITokenService`, `IActivityLogRepository`, `ITeamRepository`, `ITicketRepository`, `IUserRepository`, `IAttachmentRepository`, `ICommentRepository`, and `IResult`.

## update - 2026-06-15 - Code format fix (#36)
**Goal:** Enforce consistent code formatting across the project.
**Description:** Ran automated code formatting to fix style inconsistencies throughout the codebase.

## update - 2026-05-24 - Project structure refactor (#35)
**Goal:** Add required project documentation and configuration files.
**Description:** Added `CHANGELOG.md`, `LICENSE`, `README.md`, `CONTRIBUTING.md`, `SECURITY.md`, `CODEOWNERS`, `.editorconfig`, `.env.example`, and `.gitattributes`. Added ADR documents (`ADR-000-template`, `ADR-001-tech-stack`) under `docs/adr/`. Added build, format, and test PowerShell scripts under `scripts/`. Configured editorconfig EOL settings.

## update - 2026-03-09 - Exceptions cleanup and mapper trim (#34)
**Goal:** Remove unused exception classes and mappings now superseded by the Result pattern.
**Description:** Deleted `AlreadyExistsException`, `ForbiddenException`, `InvalidArgumentException`, and `NotFoundException` from the Domain layer. Removed the corresponding entries from `ExceptionProblemDetailsMapper`.

## update - 2026-03-09 - Result pattern across handlers and controllers (#33)
**Goal:** Replace exception-driven flow with a Result-based pattern throughout the Application layer.
**Description:** Updated all command and query handlers (auth, team, ticket, user) to return `Result<T>` instead of throwing exceptions. Added `ResultExtensions` and `ErrorExtensions` helpers for controller-side `Match` dispatch. Updated all five controllers to use the Result match pattern. Converted 81 files (952 insertions / 698 deletions) including handlers, commands, queries, and domain types.

## update - 2026-03-06 - Result pattern for negative transition tests (#32)
**Goal:** Align domain unit tests with the Result pattern introduced in the domain layer.
**Description:** Refactored `TicketStatusNegativeTransitions` tests to assert on `Result` objects instead of catching exceptions. Extracted shared result assertion logic into a helper.

## update - 2026-03-06 - Domain ticket transitions use Result (#31)
**Goal:** Remove exception throws from domain ticket state-transition methods.
**Description:** Replaced all `throw` statements inside `Ticket.cs` state-machine methods with `Result.Failure(...)` returns. Cleaned up unnecessary empty lines and whitespace.

## update - 2026-03-06 - Error hierarchy and Result base (#30)
**Goal:** Establish a reusable domain-level error and Result abstraction.
**Description:** Added `Error` base class with derived types `ForbiddenError`, `AlreadyExistsError`, `InvalidArgumentError`, and `NotFoundError` under `Tickefy.Domain/Common/Errors/`. Introduced `IResult` and generic `Result<T>` with `Success`/`Failure` factory methods and a `Match` method under `Tickefy.Domain/Common/Results/`.

## update - 2026-02-19 - Ticket actions and publish flow (#25 / TICK-8)
**Goal:** Complete the ticket workflow with draft creation, publish, fail, accept, and start-work actions.
**Description:** Added `CreateDraftTicketCommand` + handler and validator, `PublishTicketCommand` + handler + tests, `FailTicketCommand` + handler + validator, `AcceptTicketCommand` + handler, and `StartWorkTicketCommand` + handler. Exposed corresponding endpoints in `TicketController`. Renamed `Action.cs` → `TicketAction.cs`. Added publish unit tests and create-draft unit tests.

## update - 2026-02-17 - Ticket action refactor (#TICK-7)
**Goal:** Stabilize ticket workflow handlers and introduce reopen support.
**Description:** Added `ReopenTicketCommand` + handler + validator. Refactored `CancelTicketCommandHandler`, `CompleteTicketCommandHandler`, `TakeTicketCommandHandler`, and `GetTicketByIdQueryHandler`. Added `ActionHelper` for shared action-eligibility logic and introduced `ReasonForTicketActionRequest` DTO. Added EF Core–backed test handlers for cancel, complete, reopen, take, and `GetTicketById` under `tests/`.

## update - 2026-02-16 - Add member / remove member domain fixes (#TICK-14)
**Goal:** Fix domain entity relationships for team membership operations.
**Description:** Updated `Team` and `User` entities to correctly reflect membership navigation properties required by `AddMember` and `RemoveMember` handlers.

## update - 2026-02-14 - Application-layer unit tests (#TICK-9)
**Goal:** Add handler-level unit tests for the Application project.
**Description:** Created `Tickefy.Application.Tests` project with tests for `GetTicketByIdQueryHandler`, `CanExecute` positive/negative scenarios, and `RequireReason` validation. Renamed `ActionResponse` → `TicketActionResponse` and `ActionResult` → `TicketActionResult` for clarity.

## update - 2026-02-11 - Ticket actions responses and permissions fixes (#25)
**Goal:** Stabilize ticket action results and access rules.
**Description:** Added `TicketDetailsResult`, `ActionResult`, and `ActionHelper` to expose available actions per ticket state. Fixed role-based access control (removed admin access from accept/reopen/publish). Added `ActionResponse` to the API response model and mapped it via `TicketMappingProfile`.

## update - 2026-02-08 - Ticket test builder and negative tests (#24)
**Goal:** Expand ticket test coverage and improve test tooling.
**Description:** Added `Tickefy.Domain.Tests` project with `TicketBuilder`, `TicketCreationTests`, `TicketStatusPositiveTransitions`, and `TicketStatusNegativeTransitions` test suites. Added to solution file.

## update - 2026-02-04 - Ticket actions and publish flow (#23)
**Goal:** Add ticket action model and publish domain behavior.
**Description:** Added `Action` domain model and `GetAvailableActions` logic to `Ticket`. Implemented `Publish` domain method with required field validation. Fixed `Take` and `Revise` handler namespaces and updated `Status` enum. Moved exception types to the correct project.

## update - 2025-12-11 - Agent ticket visibility fix (#22)
**Goal:** Ensure agents can view accepted tickets assigned to them.
**Description:** Updated the ticket query filter so that agents can see tickets in `Accepted` status that are assigned to them.

## update - 2025-12-11 - Persist logs on assignment (#21)
**Goal:** Fix missing activity log entries when a ticket is assigned.
**Description:** Ensured the activity log record is saved when ticket assignment occurs.

## update - 2025-12-11 - Activity log ordering (#20)
**Goal:** Make activity log results consistently ordered.
**Description:** Added explicit `OrderByDescending(l => l.Created)` when fetching activity log entries.

## update - 2025-12-11 - AI service logging and config updates (#19)
**Goal:** Improve AI service observability and configuration correctness.
**Description:** Added structured logging to `AiService` and updated the AI-related defaults in `appsettings.json`.

## update - 2025-12-11 - Add-member validation fixes (#18)
**Goal:** Correct add-member request validation and normalize handler return types.
**Description:** Fixed validation rules on the `AddMemberRequest`. Made `AddMemberCommand` and `RemoveMemberCommand` return `Unit` for consistency.

## update - 2025-12-09 - My teams query fixes (#17)
**Goal:** Return the correct teams for the current member.
**Description:** Fixed `GetTeamByUserIdQuery` initialization and the member-filter predicate. Cleaned up variable naming after review.

## update - 2025-12-10 - Get user by login support (#16)
**Goal:** Add user lookup by login to enable login-based team member addition.
**Description:** Added `GetByLoginAsync` to `IUserRepository` and its EF implementation. Removed a dedicated get-by-login endpoint in favor of embedding the lookup inside the `AddMember` handler.

## update - 2025-12-07 - Swagger documentation for API (#15)
**Goal:** Document API endpoints in Swagger.
**Description:** Added `SwaggerOperation` annotations to all ticket, team, user, and auth endpoints. Fixed the queue endpoint route and resolved `TicketController` compilation issues found during review.

## update - 2025-12-06 - Response DTO fixes (#14)
**Goal:** Align API responses with expected client contracts.
**Description:** Corrected property names in response DTOs. Improved result-to-response mapping for team and ticket endpoints.

## update - 2025-12-06 - Ticket assignment feature (#12)
**Goal:** Enable ticket assignment to agents and teams.
**Description:** Added ticket assignment domain behavior and the corresponding handler for assigning an agent or team to a ticket.

## update - 2025-11-27 - DB-first user approach (#13)
**Goal:** Introduce DB-first persistence for user data.
**Description:** Added EF Core DB-first mapping for the `User` entity with a `UserId` strongly-typed ID conversion fix.

## update - 2025-11-27 - Team CRUD and log view (#11)
**Goal:** Add team management features and activity log visibility.
**Description:** Implemented create, read, and delete team commands/queries and handlers. Added `GetMyTeam` and `GetAllTeams` queries. Added activity log view for teams. Included team integration tests.

## update - 2025-11-26 - User CRUD follow-up fixes (#9)
**Goal:** Stabilize user CRUD after initial merge.
**Description:** Applied follow-up fixes to the user CRUD flow (namespace cleanup, unused reference removal).

## update - 2025-11-26 - User CRUD and profile updates (#8)
**Goal:** Implement user management features.
**Description:** Added `GetAllUsers` (admin only), `GetUserById`, `DeleteUser`, `SetUserRole`, `UpdateProfile`, and `SetPassword` commands/queries and handlers. Exposed corresponding endpoints in `UserController`.

## update - 2025-11-25 - Ticket cancel flow (#7)
**Goal:** Allow users to cancel tickets.
**Description:** Added `CancelTicketCommand` + handler and exposed `PUT /tickets/{id}/cancel` in `TicketController`.

## update - 2025-11-25 - Ticket revise flow (#6)
**Goal:** Allow ticket revision after submission.
**Description:** Implemented `ReviseTicketCommand` + handler and exposed the revise endpoint. Fixed related error types and namespace issues.

## update - 2025-11-25 - Ticket completion flow (#5)
**Goal:** Allow tickets to be marked complete.
**Description:** Added `CompleteTicketCommand` + handler and exposed `PUT /tickets/{id}/complete` in `TicketController`. Cleaned up namespace imports and error messages.

## update - 2025-11-25 - Audit log feature (#4)
**Goal:** Track ticket activity history.
**Description:** Added `ActivityLog` domain entity, `IActivityLogRepository`, and EF implementation. Wired log creation into ticket-lifecycle handlers. Added `ActivityLogController` with paginated and per-ticket query endpoints.

## update - 2025-11-25 - Ticket registration and comments (#3)
**Goal:** Enable ticket creation and discussion threads.
**Description:** Added `CreateTicketCommand` + handler with AI-driven category and priority suggestions via `IAiService`. Added `PostCommentCommand` + handler. Fixed mapping profile and post-review issues.

## update - 2025-11-23 - Authentication foundation (#2)
**Goal:** Add authentication endpoints and core auth logic.
**Description:** Implemented `RegisterUserCommand`, `LoginUserCommand`, and `SetPasswordCommand` with handlers. Added `AuthController` with register, login, and set-password endpoints. Cleaned up namespaces.

## update - 2025-11-20 - Global error handling (#1)
**Goal:** Establish consistent API error handling patterns.
**Description:** Added domain exception types and a global `ExceptionHandler` middleware. Introduced `ExceptionProblemDetailsMapper` to convert exceptions to RFC 7807 `ProblemDetails` responses. Removed the obsolete `ProblemDetailsException` usage.
