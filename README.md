# ShipFlow

[![CI](https://github.com/SalvaMiraMori/shipflow/actions/workflows/ci.yml/badge.svg)](https://github.com/SalvaMiraMori/shipflow/actions/workflows/ci.yml)

An order and shipment platform built as a **cloud-native .NET 10 microservices** reference project:
Clean Architecture + DDD services, event-driven messaging with Azure Service Bus, orchestrated locally
with .NET Aspire, deployed to Azure Container Apps with Bicep, and fronted by an Angular SPA.

> **Status:** Phase 1 in progress — Ordering service implemented (spec 001). See the [roadmap](#roadmap).

## Architecture

```
Angular SPA ──► YARP API Gateway ──► Ordering API   (Azure SQL, Clean Arch + DDD)
                                 ├─► Inventory API  (PostgreSQL)
                                 └─► Shipping API   (PostgreSQL, carrier selection)
                                           │
              Azure Service Bus (topics) ◄─┴─► Notification Function (Azure Functions)
```

**Order flow (event-driven, Phase 3):**

```
OrderPlaced ─► StockReserved ─► ShipmentRequested ─► ShipmentCreated ─► CustomerNotified
          └──► StockReservationFailed ─► OrderCancelled (saga compensation)
```

## Tech stack

| Area | Choice |
|---|---|
| Runtime | .NET 10 (LTS), C# latest |
| Local orchestration | .NET Aspire (dashboard, service discovery, distributed traces) |
| Services | ASP.NET Core minimal APIs, EF Core, Clean Architecture, DDD |
| Messaging | MassTransit — RabbitMQ locally, Azure Service Bus in the cloud |
| Gateway | YARP |
| Cloud | Azure Container Apps, Azure SQL, Key Vault + Managed Identity, Application Insights (OpenTelemetry) |
| IaC | Bicep |
| Containers | Docker, Kubernetes (kind locally, optional AKS) |
| Frontend | Angular (standalone components, signals), SignalR |
| Quality | xUnit v3, Testcontainers, SonarCloud, GitHub Actions |

## Repository layout

```
src/
  AppHost/                 .NET Aspire orchestrator — `dotnet run` starts the whole system
  ServiceDefaults/         OpenTelemetry, health checks, resilience, service discovery
  BuildingBlocks/          Shared Core libraries (DDD, specifications, CQRS handlers, EF repository, Problem Details)
    Core.Domain  Core.Specification  Core.SpecificationEF  Core.Application
    Core.Persistence  Core.Infrastructure  Core.Presentation
  Services/
    Ordering/              One folder per microservice, each with the same seven layers:
      ShipFlow.Ordering.Domain          aggregates, value objects, specifications, domain errors
      ShipFlow.Ordering.Application     commands / queries / handlers
      ShipFlow.Ordering.Contracts       HTTP request & response DTOs
      ShipFlow.Ordering.Infrastructure  current user, CORS, external clients
      ShipFlow.Ordering.Persistence     EF Core (SQL Server), repository, migrations
      ShipFlow.Ordering.Presentation    controllers, routes, mappings
      ShipFlow.Ordering.Api             composition root (Program.cs, Dockerfile)
  Gateway/                 YARP API gateway (Phase 2)
  Web/                     Angular SPA (Phase 5)
infra/                     Bicep templates
tests/
  BuildingBlocks/Core.Tests
  Services/Ordering/       Domain.Tests, Application.Tests, Api.IntegrationTests (Testcontainers)
docs/
  specs/                   Feature specs written before implementation
  adr/                     Architecture decision records (layout & Core: ADR 0002)
```

**Request flow:** `OrdersController` → `ICommandHandler` (Logging → Validation → UnitOfWork decorators) →
`Order.Place(...)` (domain rules, `ErrorOr`) → `IRepository<Order>` → EF Core → SQL Server; errors become RFC 9457
Problem Details.

## Getting started

**Prerequisites:** .NET 10 SDK, Docker Desktop (or Podman).

```bash
dotnet build ShipFlow.slnx
dotnet test ShipFlow.slnx
dotnet run --project src/AppHost      # SQL Server container + Ordering API + Aspire dashboard
```

Try it with `src/Services/Ordering/ShipFlow.Ordering.Api/ShipFlow.Ordering.Api.http`. Integration tests need Docker.

## Roadmap

- [x] **Phase 0 — Foundation:** solution layout, Aspire AppHost + ServiceDefaults, CI, specs workflow
- [ ] **Phase 1 — Ordering API on Azure:** Clean Architecture + DDD (✓ layered service + Core building blocks), Bicep, Container Apps, Key Vault via Managed Identity, App Insights
- [ ] **Phase 2 — Microservices:** Inventory + Shipping (own databases), YARP gateway, distributed tracing
- [ ] **Phase 3 — Async messaging:** Service Bus, transactional outbox, idempotent consumers, DLQ, order saga, Azure Function
- [ ] **Phase 4 — Kubernetes:** Dockerfiles, manifests, probes, kind (+ optional AKS/Helm)
- [ ] **Phase 5 — Angular frontend:** catalog, checkout, live order tracking (SignalR), OIDC login
- [ ] **Phase 6 — Polish:** SonarCloud gate, Dependabot, k6 load test, ADRs, demo GIF

## How I use Claude Code

Every feature starts as a short spec in [`docs/specs/`](docs/specs) (problem, acceptance criteria, test cases).
Claude Code implements against the spec following [`CLAUDE.md`](CLAUDE.md); I review every change, and
tests are written first. The spec, the tests and the reviewed diff are all in the history of each PR.

## License

[MIT](LICENSE)
