# Changelog

Format: update - date - name (#number)

## update - 2026-06-21 - Documentation improvement (#37)
**Goal:** Improve project documentation with endpoint docs and interface summaries.
**Description:** Added endpoint documentation and XML summary comments for interfaces across the codebase.

## update - 2026-06-15 - Code format fix (#36)
**Goal:** Enforce consistent code formatting across the project.
**Description:** Ran code formatting to fix style inconsistencies throughout the codebase.

## update - 2026-05-24 - Project structure refactor (#35)
**Goal:** Add required project documentation and configuration files.
**Description:** Added changelog, licence, and other required documents, configured editorconfig EOL settings, and added copyright to the licence file.

## update - 2026-03-09 - Exceptions cleanup and mapper trim (#34)
**Goal:** Remove unused exception classes and mappings.
**Description:** Removed unused exception types and pruned exception-to-problem mapping in the exception mapper.

## update - 2026-03-09 - Result pattern across handlers and controllers (#33)
**Goal:** Replace exception-driven flow with a Result-based pattern.
**Description:** Introduced Result match helpers and ProblemDetails mapping, updated handlers/controllers to return Result, and adjusted related tests and response mappings.

## update - 2026-03-06 - Result pattern for negative transition tests (#32)
**Goal:** Align negative transition tests with the Result pattern.
**Description:** Updated negative transition test cases to use Result assertions and simplified test helpers.

## update - 2026-02-11 - Ticket actions responses and permissions fixes (#25)
**Goal:** Stabilize ticket action results and access rules.
**Description:** Updated ticket action result mapping, corrected available actions and permission checks, and fixed related response models.

## update - 2026-02-08 - Ticket test builder and negative tests (#24)
**Goal:** Expand ticket test coverage and tooling.
**Description:** Added a ticket test builder, organized tests into categories, introduced negative transition tests, and created a domain test project.

## update - 2026-02-04 - Ticket actions and publish flow (#23)
**Goal:** Add ticket action model and publish domain behavior.
**Description:** Added ticket actions and available action calculation, implemented publish domain method, and aligned handlers with the new action flow.

## update - 2025-12-11 - Agent ticket visibility fix (#22)
**Goal:** Ensure agents can view accepted tickets.
**Description:** Updated ticket queries so agents can see accepted tickets assigned to them.

## update - 2025-12-11 - Persist logs on assignment (#21)
**Goal:** Fix missing activity log entries on assignment.
**Description:** Ensured activity logs are saved when ticket assignment occurs.

## update - 2025-12-11 - Activity log ordering (#20)
**Goal:** Make activity logs consistently ordered.
**Description:** Added explicit ordering when fetching activity logs.

## update - 2025-12-11 - AI service logging and config updates (#19)
**Goal:** Improve AI service observability.
**Description:** Added AI service logging and updated related configuration defaults.

## update - 2025-12-11 - Add-member validation fixes (#18)
**Goal:** Correct add-member request validation and return types.
**Description:** Fixed add-member request validation and normalized handler return values.

## update - 2025-12-10 - Get user by login support (#16)
**Goal:** Add user lookup by login for team workflows.
**Description:** Added user-by-login lookup and adjusted team member add flow to use login-based resolution.

## update - 2025-12-09 - My teams query fixes (#17)
**Goal:** Return correct teams for members.
**Description:** Fixed my teams query initialization and member filtering, with cleanup after review.

## update - 2025-12-07 - Swagger documentation for API (#15)
**Goal:** Document API endpoints in Swagger.
**Description:** Added Swagger annotations for ticket endpoints and fixed controller/queue issues found during review.

## update - 2025-12-06 - Response DTO fixes (#14)
**Goal:** Align API responses with expected contracts.
**Description:** Corrected response DTO property names and improved result mapping for endpoints.

## update - 2025-12-06 - Ticket assignment feature (#12)
**Goal:** Enable ticket assignment flow.
**Description:** Added ticket assignment behavior for agents/teams.

## update - 2025-11-27 - DB-first user approach (#13)
**Goal:** Introduce DB-first persistence for user data.
**Description:** Added DB-first user mapping and persistence changes.

## update - 2025-11-27 - Team CRUD and log view (#11)
**Goal:** Add team management and log visibility.
**Description:** Implemented team CRUD, team tests, and activity log view for teams.

## update - 2025-11-26 - User CRUD follow-up fixes (#9)
**Goal:** Stabilize user CRUD after initial merge.
**Description:** Applied follow-up fixes to the user CRUD flow.

## update - 2025-11-26 - User CRUD and profile updates (#8)
**Goal:** Implement user management features.
**Description:** Added user CRUD, admin user listing, and profile update/password reset endpoints.

## update - 2025-11-25 - Ticket cancel flow (#7)
**Goal:** Allow users to cancel tickets.
**Description:** Added ticket cancel endpoint and updated ticket controller logic.

## update - 2025-11-25 - Ticket revise flow (#6)
**Goal:** Allow ticket revision after submission.
**Description:** Implemented ticket revise endpoint and fixed errors and namespaces related to the flow.

## update - 2025-11-25 - Ticket completion flow (#5)
**Goal:** Allow tickets to be marked complete.
**Description:** Added ticket completion endpoint and cleaned up error messaging.

## update - 2025-11-25 - Audit log feature (#4)
**Goal:** Track ticket activity history.
**Description:** Added audit log entity and related logging behavior.

## update - 2025-11-25 - Ticket registration and comments (#3)
**Goal:** Enable ticket creation and discussion.
**Description:** Added ticket creation with AI category/priority suggestions and comment posting, with mapping fixes.

## update - 2025-11-23 - Authentication foundation (#2)
**Goal:** Add auth endpoints and core auth logic.
**Description:** Implemented auth controllers and core auth flow with namespace cleanup.

## update - 2025-11-20 - Global error handling (#1)
**Goal:** Establish API error handling patterns.
**Description:** Added exceptions and a global error handler, removing obsolete ProblemDetailsException usage.
