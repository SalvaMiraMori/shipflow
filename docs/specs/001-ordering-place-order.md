# 001 — Place order (Ordering API)

- **Status:** Implemented — tick the criteria once `dotnet test` is green locally
- **Phase:** 1
- **Service(s):** Ordering

## Problem
A customer needs to place an order for one or more products and get back an order id they can track.
This is the first vertical slice and the entry point of the whole order → shipment flow.

## Scope
**In:** `POST /api/orders`, `GET /api/orders/{id}`, `GET /api/orders` (paged, filter by customer/status), `POST /api/orders/{id}/cancel`; `Order` aggregate with `OrderLine`s; persistence in SQL Server
(Azure SQL in the cloud); validation; Problem Details errors.
**Out:** payment, stock checks (Phase 2/3), authentication (Phase 5).

## Design notes
- **Aggregate `Order`:** `OrderId`, `CustomerId`, `Status` (`Placed`), `ShippingAddress` (value object),
  `Lines` (`ProductId`, `Quantity`, `UnitPrice: Money`), `Total` (derived), `PlacedAt`.
- **Invariants:** at least one line; quantity > 0; same currency across all lines; no duplicate `ProductId`
  (merge quantities instead).
- Raises domain event `OrderPlaced` (becomes an integration event in Phase 3).
- `POST /orders` → `201 Created` with `Location: /orders/{id}`; invalid input → `400` Problem Details with
  field errors; `GET` unknown id → `404`.

## Acceptance criteria
- [ ] Given a valid request, when `POST /orders`, then `201` and the order can be fetched with `GET /orders/{id}`.
- [ ] Given a request with no lines, then `400` with a validation error on `lines`.
- [ ] Given a line with quantity ≤ 0, then `400`.
- [ ] Given lines in different currencies, then `400`.
- [ ] Given two lines with the same product, then a single line with the summed quantity is stored.
- [ ] The order total equals the sum of `quantity × unitPrice`.

## Test cases
| # | Type | Scenario | Expected |
|---|---|---|---|
| 1 | unit | `Order.Place` with valid lines | status `Placed`, `OrderPlaced` raised, total correct |
| 2 | unit | `Order.Place` with no lines | domain exception |
| 3 | unit | mixed currencies | domain exception |
| 4 | unit | duplicate product | lines merged |
| 5 | integration (Testcontainers SQL) | POST then GET | round-trip equal |
| 6 | integration | invalid payload | 400 Problem Details |

## Decisions taken during implementation
- Strongly-typed ids (`OrderId`, `CustomerId`, `ProductId`, `OrderLineId`) as `EntityId`/`AggregateRootId` classes with EF value converters.
- The same product twice with **different** unit prices is rejected (`Order.Lines.ConflictingUnitPrice`) instead of guessing which price wins.
- Domain rules return `ErrorOr` errors instead of throwing exceptions (EcoGreen pattern); the controller maps them to Problem Details.
- Routes follow the `api/[controller]` convention of the base `ApiController`.

## Implementation map
| Test case | Test |
|---|---|
| 1–4 | `tests/Services/Ordering/ShipFlow.Ordering.Domain.Tests/Orders/OrderTests.cs` |
| 5–6 | `tests/Services/Ordering/ShipFlow.Ordering.Api.IntegrationTests/OrdersEndpointsTests.cs` (Testcontainers SQL Server) |
