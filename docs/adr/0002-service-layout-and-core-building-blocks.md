# ADR 0002 — Service layout and Core building blocks (EcoGreen pattern)

- **Status:** Accepted
- **Date:** 2026-11-12

## Context
I already build backends at work (EcoGreen) with a layered Clean Architecture: `Domain`, `Application`, `Contracts`,
`Infraestructure`, `Persistence`, `Presentation` and an `App` composition root, sharing a set of `Core.*` libraries
(DDD base classes, a fork of Ardalis.Specification, generic repositories). ShipFlow should reuse that structure so the
portfolio shows the way I really work, while fixing the weaknesses found in Core along the way.

## Decision
**Per service, seven projects** under `src/Services/<Service>/` (EcoGreen layout, one copy per microservice):

| Project | Responsibility | References |
|---|---|---|
| `ShipFlow.<S>.Domain` | Aggregates, value objects, smart enums, validators, domain errors, specifications, filters | Core.Domain, Core.Specification |
| `ShipFlow.<S>.Application` | Commands, queries, handlers, command validators, domain event handlers | Domain, Core.Application |
| `ShipFlow.<S>.Contracts` | HTTP request/response DTOs (and integration events from Phase 3) | — |
| `ShipFlow.<S>.Infrastructure` | Current user, CORS, external HTTP clients | Application, Core.Infrastructure |
| `ShipFlow.<S>.Persistence` | DbContext, EF configurations, repository, migrations, DB initializer | Application, Core.Persistence |
| `ShipFlow.<S>.Presentation` | Controllers, routes, contract ↔ command/response mappings | Application, Contracts, Core.Presentation |
| `ShipFlow.<S>.Api` | Composition root: `Program.cs`, configuration, Dockerfile | all of the above + ServiceDefaults |

Each layer exposes one `Add<Layer>()` extension, and `Program.cs` only chains them (as in EcoGreen).

**Core lives in the repo** as `src/BuildingBlocks/Core.*`, referenced by `ProjectReference` (not a private NuGet feed),
upgraded from .NET 8 to .NET 10. Changes compared with the EcoGreen/BaseBE Core:

| Area | Before | Now |
|---|---|---|
| Specification | `Core.SpecificationEF` mixed the pattern with EF Core, so every Domain referenced EF Core | Split: `Core.Specification` (no EF, used by Domain) + `Core.SpecificationEF` (EF evaluators) |
| Query tags | Each aggregate wrote its own `TagWith` and the app subclassed the evaluator | Generic `TagWith<T>()` + `QueryTagEvaluator` in the default pipeline |
| Aggregate root | Re-declared `Id` with `new` and skipped the base constructor → base `Id` null, `Equals`/`GetHashCode` threw | Single `Id` set through `Entity<TId>(id)`; regression test in `Core.Tests` |
| Ids | `AggregateRootId` implemented `IAggregateRoot` | `EntityId<T>` / `AggregateRootId<T> : IAggregateRootId` |
| Value objects | XOR hash (collides on swapped components, throws with no components) | `HashCode`-based, null-safe |
| Enumerations | Two near-identical classes; every enum re-implemented `FromValue`/`FromName` with ErrorOr | One `Enumeration<TEnum, TValue>` with `FromValue`/`FromName` returning `ErrorOr` |
| Messaging | Core had MediatR interfaces (unused by EcoGreen, which had its own handlers) | EcoGreen's `ICommandHandler`/`IQueryHandler` in Core; no MediatR (licence) |
| Pipeline | Only a UnitOfWork decorator, registered by hand per project | `AddCoreApplication()`: Logging → Validation (FluentValidation) → UnitOfWork decorators |
| Domain events | Raised but never dispatched; `IDomainEvent : INotification` (MediatR) | `DomainEvent` record + `IDomainEventDispatcher`, dispatched by the unit of work after commit |
| Repositories | 4 overlapping read interfaces, `SaveChanges` on repositories, swallowed exceptions, `ToBlockingEnumerable`, Mapster inside persistence | `IReadRepository<T>`, `IWriteRepository<T>`, `IRepository<T>` (aggregate roots only); projections through `ISpecification<T, TResult>`; exceptions bubble up |
| Unit of work | `IUnitOfWork` exposed `IDbContextTransaction`; disposed a DbContext it didn't own; old `BaseRepository.SaveChangesAsync` was fire-and-forget | EF-free abstraction; auditing, soft delete and domain events in `EfUnitOfWork<TContext>` |
| Dates | Custom `IDateTimeProvider` | `System.TimeProvider` (`FakeTimeProvider` in tests) |
| Validation helpers / error templates | Copied into every Domain; string helpers threw on `null` | `Core.Domain.Validation` + `ErrorTemplates`, null-safe; `ValidateToErrorOr()` replaces the repeated `ConvertAll(...)` block |
| Pagination | Two page models (domain + contracts) with diverging maths | One `Pagination` (clamped); `PagedResponse` built from it |
| ApiController | In each Presentation | `Core.Presentation.ApiController`, adds `errorCodes` to Problem Details and hides 500 details |
| ApiClient / Healthy / Infraestructure | Three tiny projects; health check was decompiled code with `dynamic`; ASP.NET Core 2.2 packages | Merged into `Core.Infrastructure` |
| Soft delete | Fields only | `ApplySoftDeleteQueryFilter()` + deletes converted by the unit of work |

Deliberate differences from EcoGreen in the services: English `Infrastructure` spelling, explicit mapping extension
methods instead of Mapster (compile-time checked), application commands separate from HTTP contracts, empty lists
return `200` with an empty page instead of `204`, and OpenTelemetry (ServiceDefaults) instead of Serilog.

## Consequences
- A new service is a copy of the Ordering folder structure; the patterns are documented in `CLAUDE.md`.
- Core can be packed as NuGet later (`dotnet pack src/BuildingBlocks`) if EcoGreen wants to consume the improvements.
- `Core.Specification` and `Core.SpecificationEF` keep upstream Ardalis code style, so they opt out of
  `TreatWarningsAsErrors`/code-style enforcement; every other project keeps the strict settings.
- In Azure, EF migrations run as a pipeline step; `DbInitializerService` only migrates automatically in Development.
