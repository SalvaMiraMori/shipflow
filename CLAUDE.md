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
- Each service: `src/Services/<Name>/<Name>.Domain | .Application | .Infrastructure | .Api`.
  Dependencies point inwards: Api → Application → Domain; Infrastructure → Application.
  **Domain has no references to EF Core, ASP.NET or MassTransit.**
- DDD: aggregates protect their invariants; no public setters on entities; use value objects
  (e.g. `Money`, `Quantity`, `Address`) instead of primitives. Domain events raised by aggregates.
- Each service owns its database. No cross-service DB access, no shared EF DbContext.
- Services talk asynchronously through integration events (Phase 3+). Integration event contracts live in
  a `<Service>.Contracts` project; they are immutable records and versioned additively.
- APIs: minimal APIs, `TypedResults`, Problem Details (RFC 9457) for errors, FluentValidation for input.
- Every service calls `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()`.
- Configuration and secrets: never commit secrets. Local = user-secrets / Aspire; cloud = Key Vault via
  Managed Identity.

## Code conventions
- File-scoped namespaces, nullable enabled, warnings are errors (see `Directory.Build.props`).
- NuGet versions only in `Directory.Packages.props` (Central Package Management) — never in a csproj.
- Async all the way; pass `CancellationToken` through.
- Tests: xUnit v3 + Shouldly. Names: `Method_State_ExpectedResult`. Integration tests use Testcontainers,
  not in-memory providers.

## Don'ts
- Don't add packages without stating why in the PR.
- Don't bump the target framework or Aspire SDK version unprompted.
- Don't create Azure resources by hand — everything is Bicep in `infra/`.
