# ShipFlow

[![CI](https://github.com/SalvaMiraMori/shipflow/actions/workflows/ci.yml/badge.svg)](https://github.com/SalvaMiraMori/shipflow/actions/workflows/ci.yml)

An order and shipment platform built as a **cloud-native .NET 10 microservices** reference project:
Clean Architecture + DDD services, event-driven messaging with Azure Service Bus, orchestrated locally
with .NET Aspire, deployed to Azure Container Apps with Bicep, and fronted by an Angular SPA.

> **Status:** Phase 0 — foundation. See the [roadmap](#roadmap).

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
  AppHost/           .NET Aspire orchestrator — `dotnet run` starts the whole system
  ServiceDefaults/   OpenTelemetry, health checks, resilience, service discovery
  Services/          Ordering, Inventory, Shipping, Notification (one folder each)
  Gateway/           YARP API gateway
  Web/               Angular SPA
infra/               Bicep templates
tests/               Unit, integration and architecture tests
docs/
  specs/             Feature specs written before implementation
  adr/               Architecture decision records
```

## Getting started

**Prerequisites:** .NET 10 SDK, Docker Desktop (or Podman).

```bash
dotnet build ShipFlow.slnx
dotnet test ShipFlow.slnx
dotnet run --project src/AppHost      # opens the Aspire dashboard
```

## Roadmap

- [x] **Phase 0 — Foundation:** solution layout, Aspire AppHost + ServiceDefaults, CI, specs workflow
- [ ] **Phase 1 — Ordering API on Azure:** Clean Architecture + DDD, Bicep, Container Apps, Key Vault via Managed Identity, App Insights
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
