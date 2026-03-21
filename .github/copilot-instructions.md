# VetPharmacy — Project Instructions

This document defines the engineering, architecture, and security standards for the VetPharmacy API. All contributors must follow these guidelines when adding or modifying code.

## Code Style & Conventions

- **Language Standards:** Follow C# conventions and ASP.NET Minimal API patterns.
- **Endpoint Structure:** Endpoint handlers must be static extension methods on `RouteGroupBuilder`.
- **Asynchronous Flow:** Always use `async`/`await` and provide a `CancellationToken` to all internal calls.
- **Abstraction:** Never expose Entity Framework (EF) entities directly to the API layer.
- **Defensive Naming:** Use variable names that describe security context (e.g., `untrustedInput` for raw request data vs. `sanitizedData` for validated objects) to prevent **Injection (A03:2021)**.

## Architecture

- **Pattern:** Feature-based **Vertical Slice Architecture**.
- **Feature Isolation:** Each feature must contain its own Endpoint, DTOs, Validation logic, and Tests.
- **Shared Logic:** Maintain common utilities, infrastructure, and core security logic in `Common/`, `Infrastructure/`, and `Security/` directories.
- **Tech Stack:** ASP.NET Minimal APIs, EF Core, PostgreSQL, and Keycloak for Identity Management.

## Security Rules (OWASP Top 10 Aligned)

1. **Explicit Authorization (A01:2021):** Every endpoint must explicitly define access levels using `.RequireAuthorization(...)` with specific policies or `.AllowAnonymous()`.
2. **DTO Separation (A04:2021):** Use operation-specific DTOs for every request (e.g., separate DTOs for `CreateProduct` vs. `UpdateProduct`) to prevent **Mass Assignment**.
3. **Identity Integrity (A07:2021):** User identity must be sourced exclusively from validated JWT claims (Email or Sub) via a secure claims transformer.
4. **Fail-Closed Design:** Structure conditional logic to deny access or throw an exception by default. Guard clauses should exit early if security criteria are not met.
5. **Secure Error Handling (A09:2021):** Return **RFC 7807 ProblemDetails**. Ensure stack traces and internal exception details are stripped in production.
6. **Query Safety (A03:2021):** Always limit database queries with a maximum page size of 100. Use parameterized queries (standard in EF Core) to prevent SQL injection.
7. **Logging Security (A09:2021):** Never log JWT tokens, secrets, passwords, or PII. Allowed identifiers: `UserId`, `CorrelationId`, and `ResourceId`.

## Validation & Engineering Principles

- **Security by Default:** Systems must be secure in their initial state without manual intervention. This includes enforcing secure headers (HSTS, CSP), using secure-by-design defaults for all libraries, and ensuring all new features start with the most restrictive permissions.
- **Mandatory Validation:** All incoming DTOs must be validated (e.g., FluentValidation).
- **Resource Protection:** Implement rate limiting for high-traffic or sensitive endpoints like `login`, `search`, and public-facing routes.
- **Principle of Least Privilege:** Code should only request the minimum data or permissions required for its immediate task.
- **Supply Chain Security:** Regularly audit NuGet packages for known vulnerabilities (e.g., `dotnet list package --vulnerable`).

## Testing Requirements

Every vertical slice must include tests covering:

- **Authorized:** The "Happy Path" with valid credentials.
- **Unauthorized:** Verification that missing/expired tokens are rejected.
- **Forbidden:** Verification that valid users with insufficient permissions are blocked.
- **Invalid Input:** Boundary testing and payload sanitization checks.

## Build & Environment

- **Build Command:** `msbuild /property:GenerateFullPaths=true /t:build /consoleloggerparameters:NoSummary`.
- **Test Command:** `dotnet test VetPharmacy.Api.Tests`.
- **Local Dev:** `docker-compose up` (PostgreSQL on 5432, Keycloak on 8080).
