# ADR 0001 — Record architecture decisions

- **Status:** Accepted
- **Date:** 2026-11-12

## Context
This project makes several non-obvious choices (Aspire, Container Apps vs AKS, outbox, MassTransit licensing).
Interviewers and future me need to know *why*, not only *what*.

## Decision
Record each significant decision as a short ADR in `docs/adr/NNNN-title.md` using the format:
Context → Decision → Consequences.

## Consequences
Decisions are reviewable in PRs and traceable in git history. Planned ADRs: Aspire for local orchestration,
Container Apps over AKS for production, transactional outbox, MassTransit v8 vs native Service Bus SDK.
