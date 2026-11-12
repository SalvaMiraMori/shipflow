# 001 — Place order (Ordering API)

- **Status:** Draft
- **Phase:** 1
- **Service(s):** Ordering

## Problem
A customer needs to place an order for one or more products and get back an order id they can track.
This is the first vertical slice and the entry point of the whole order → shipment flow.

## Scope
**In:** `POST /orders`, `GET /orders/{id}`; `Order` aggregate with `OrderLine`s; persistence in SQL Server
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

## Open questions
- Use strongly-typed ids (`OrderId : record struct`) with an EF value converter? (Proposed: yes.)
