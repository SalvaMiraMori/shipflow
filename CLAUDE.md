# CLAUDE.md — ShipFlow

Guidance for Claude Code when working in this repository.

## What this is
A portfolio-grade order & shipment platform: .NET 10 microservices (Ordering, Inventory, Shipping,
Notification) orchestrated with .NET Aspire, event-driven via MassTransit (RabbitMQ local / Azure Service Bus
cloud), deployed to Azure Container Apps with Bicep. Angular frontend in `src/Web`.

## Workflow (mandatory)
1. **Spec first.** Every feature has a spec in `docs/specs/NNN-short-name.md` (copy `000-template.md`).
   If asked to build something without a spec, draft the spec and stop for review.
2. **Tests first (TDD).** Write failing tests from the spec's acceptance criteria, then implement.
3. **Small, reviewable changes.** One spec ≈ one PR. Don't touch unrelated code.
4. **Record decisions.** Non-obvious architectural choices get an ADR in `docs/adr/`.

## Commands
```bash
dotnet build ShipFlow.slnx
dotnet test ShipFlow.slnx
dotnet format ShipFlow.slnx            # CI runs --verify-no-changes
dotnet run --project src/AppHost        # whole system + Aspire dashboard
```

## Architecture rules
- **Layout per service (EcoGreen pattern, see `docs/adr/0002`)** under `src/Services/<Name>/`:
  `ShipFlow.<Name>.Domain | .Application | .Contracts | .Infrastructure | .Persistence | .Presentation | .Api`.
  Dependencies point inwards: Presentation → Application → Domain; Infrastructure/Persistence → Application;
  Api wires everything. **Domain has no references to EF Core, ASP.NET or MassTransit** (it uses `Core.Specification`,
  not `Core.SpecificationEF`).
- Shared building blocks live in `src/BuildingBlocks/Core.*` (ProjectReference). Improve Core instead of copying code
  into a service; every Core change needs a test in `tests/BuildingBlocks/Core.Tests`.
- Each layer exposes one `Add<Layer>()` extension (`AddDomain`, `AddApplication`, `AddInfrastructure`, `AddPersistence`,
  `AddPresentation`); `Program.cs` only chains them.

### Domain (`ShipFlow.<Name>.Domain`)
- Folders mirror EcoGreen: `Application/<Aggregate>/` holds the aggregate plus `Entities/`, `Enums/`, `Events/`,
  `Filters/`, `Specifications/`, `Validators/`, `ValueObjects/`; `Common/` holds shared value objects, `Errors/`
  (`DomainErrors.<Entity>` partial classes built from `Core.Domain.Errors.ErrorTemplates`) and validators.
- Aggregates derive from `AggregateRoot<TId, TValue>`, child entities from `Entity<TId>`; no public setters; behaviour
  methods return `ErrorOr<T>` and re-validate with the aggregate's FluentValidation validator.
- Value objects: private constructor + `CreateNew(...)` factory returning `ErrorOr<T>`; ids: `Create(Guid)` /
  `CreateUnique()`. Validate with `Validator.Instance.ValidateToErrorOr(...)` / `ValidateAndCreate(...)`.
- Smart enums derive from `Enumeration<T>`; names have no `Enum` suffix (`OrderStatus.Placed`).
- Queries are specifications (`Specification<T>`, `SingleResultSpecification<T>`) composed from
  `<Aggregate>SpecificationExtensions`; compare whole value objects (`o.CustomerId.Equals(id)`) so EF can translate them;
  tag queries with `.TagWith(nameof(MySpecification))`.
- Raise domain events with `RaiseDomainEvent(new XxxDomainEvent(...))` (records, primitives only).

### Application (`ShipFlow.<Name>.Application`)
- Feature folders: `<Aggregate>/Commands/<UseCase>/` (command record, `…CommandHandler`, optional `…CommandValidator`)
  and `<Aggregate>/Queries/<UseCase>/`. Handlers implement `ICommandHandler<TCommand, TResult>` /
  `IQueryHandler<TQuery, TResult>` from `Core.Application.Messaging` and return `ErrorOr<T>`. No MediatR.
- Handlers never call `SaveChanges`: the UnitOfWork decorator commits successful commands. Input-shape validation goes
  in a FluentValidation validator for the command (run by the Validation decorator); business rules stay in the domain.
- Use `IRepository<T>` (aggregate roots) / `IReadRepository<T>` with specifications; use `TimeProvider` for "now".
- Don't catch exceptions to turn them into errors; let infrastructure failures bubble up.

### Contracts, Presentation, Persistence
- Contracts: immutable request/response records, no dependencies. Integration events (Phase 3) are added here,
  versioned additively.
- Presentation: controllers inherit `Core.Presentation.Controllers.ApiController` (route `api/[controller]`), routes in
  `Contracts/ApiRoutes.cs`, handlers injected per action with `[FromServices]`, results returned with
  `result.Match<IActionResult>(..., Problem)`. Mappings are extension methods in `<Aggregate>/Mapping/`.
- Persistence: one `IEntityTypeConfiguration` per aggregate (value converters for ids/single-value objects, owned types
  for multi-value objects and child collections), table names in `Constants/TableNames.cs`, the service repository is a
  one-line `EfRepository<T, TContext>` subclass registered with `AddCorePersistence<TContext>(typeof(Repo<>))`.
  Add migrations with `dotnet ef migrations add <Name> -p src/Services/<S>/ShipFlow.<S>.Persistence -s src/Services/<S>/ShipFlow.<S>.Api`.

### Cross-cutting
- Each service owns its database. No cross-service DB access, no shared EF DbContext.
- Services talk asynchronously through integration events (Phase 3+).
- Errors: ErrorOr → Problem Details (RFC 9457) via `ApiController`; unhandled exceptions → 500 via `UseExceptionHandler`.
- Every service calls `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()`.
- Configuration and secrets: never commit secrets. Local = user-secrets / Aspire; cloud = Key Vault via
  Managed Identity.

## Code conventions
- File-scoped namespaces, nullable enabled, warnings are errors (see `Directory.Build.props`).
- NuGet versions only in `Directory.Packages.props` (Central Package Management) — never in a csproj.
- Async all the way; pass `CancellationToken` through.
- Tests: xUnit v3 + Shouldly + NSubstitute; pass `TestContext.Current.CancellationToken`. Names:
  `Method_State_ExpectedResult`. Layout `tests/Services/<S>/ShipFlow.<S>.{Domain,Application}.Tests` and
  `.Api.IntegrationTests` (Testcontainers, not in-memory providers).

## Don'ts
- Don't add packages without stating why in the PR.
- Don't bump the target framework or Aspire SDK version unprompted.
- Don't reference `Core.SpecificationEF`, EF Core or ASP.NET from a Domain or Application project.
- Don't create Azure resources by hand — everything is Bicep in `infra/`.
