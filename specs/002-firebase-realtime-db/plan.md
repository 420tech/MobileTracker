# Implementation Plan: Firebase Realtime Database (per-user data)

**Branch**: `002-firebase-realtime-db` | **Date**: December 2, 2025 | **Spec**: [spec.md]
**Input**: Feature specification from `/specs/002-firebase-realtime-db/spec.md`

## Summary

This feature adds a REST-backed Firebase Realtime Database client to persist per-user domain data (clients, invoices, time entries, and user settings) under a per-user namespace (/users/{uid}/...). The service uses tokens emitted by the existing `IAuthService` (GetIdTokenAsync) to authenticate requests. The first iteration implements synchronous REST operations (Get/Set/Push/Delete) with comprehensive unit tests and recommended server-side security rules. Offline/queueing sync is intentionally out-of-scope for v1.

## Technical Context

Language/Version: .NET 10 (multi-target MAUI mobile app)  
Primary Dependencies: HttpClient (System.Net.Http), Moq/xUnit for tests, Microsoft.Extensions.DependencyInjection/Logging  
Storage: Firebase Realtime Database (REST API)  
Testing: xUnit unit tests (mock HttpClient via HttpMessageHandler), integration/manual tests against a Firebase project or emulator  
Target Platform: Mobile (Android/iOS/MacCatalyst/Windows) and CI test runner  
Project Type: Single multi-target MAUI app with unit tests and headless test projects  
Performance Goals: Low-latency CRUD for single resource operations; typical operations complete within 1–3s under normal network; v1 won't attempt heavy load scaling  
Constraints: Must rely on server-side security rules for data isolation; token refresh logic not implemented in v1 — serve-only short-lived session tests  
Scale/Scope: Per-user data for single-user accounts on devices; initial scope excludes offline queueing and file attachments

## Constitution Check

All required user-facing and policy checks pass for Phase 0 — spec created and reviewed. No high-risk policy violations or new infra requirements identified.

## Project Structure

This is a feature within the existing app (no new microservice). Files and artifacts added will be placed under the app and the specs folder.

Repository targets for this feature:

MobileTracker/
- Services/
  - FirebaseRealtimeDatabaseService.cs (client library, already present)
- Models/
  - Domain models for Client, Invoice, TimeEntry (new)
- ViewModels/
  - ViewModels that use IFirebaseDatabaseService for CRUD flows (updated or new)

tests/
- MobileTracker.UnitTests/
  - Services/FirebaseRealtimeDatabaseServiceTests.cs (unit tests added)
  - New unit tests for model validation and viewmodel behaviors (to be added)

Spec files:

specs/002-firebase-realtime-db/
- spec.md
- plan.md (this file)
- data-model.md (Phase 1)
- quickstart.md (Phase 1)
- contracts/ (optional API contract examples)

## Phases, Milestones & Deliverables

Phase 0 — Research & validate (complete)
- Deliverables: spec.md, checklist (done), prototype FirebaseRealtimeDatabaseService.cs, initial unit tests for service

Phase 1 — Design & data model (this iteration)
- Deliverables:
  - data-model.md listing JSON shapes for Client, Invoice, TimeEntry, UserSettings
  - quickstart.md explaining FIREBASE_DATABASE_URL usage + how to run tests against emulator or real project
  - API contract examples (optional)

Phase 2 — Implementation & tests
- Deliverables:
  - Model classes (Client, Invoice, TimeEntry) in `MobileTracker/Models`
  - Complete and robust `FirebaseRealtimeDatabaseService` implementation (already added, but refine if needed)
  - Unit tests for service (done) and additional tests for validation, error handling, token errors
  - ViewModel wiring for CRUD flows (Login already in place); add samples for Clients, Invoices, TimeEntries
  - Integration test plan and scripts for run-against-emulator (manual or CI-supported)

Phase 3 — Docs & CI
- Deliverables:
  - quickstart and developer guide in specs
  - sample Firebase security rules for per-user enforcement included in docs
  - CI integration for running service tests against emulator or mocked environment

Phase 4 — Follow-ups (optional)
- Offline-first/queueing features, attachments handling, advanced conflict resolution, long-lived connection or real-time listeners (if required)

## Task Breakdown (high-level)

1. Finalize domain model JSON shapes (data-model.md) — small (1–2 days)
2. Add domain model POCOs in code and map to Service (Client, Invoice, TimeEntry) — small (1–2 days)
3. Harden FirebaseRealtimeDatabaseService: input validation, retry handling, better error mapping — small (1–2 days)
4. Add ViewModel CRUD methods + pages (Clients CRUD, Simple Invoice create/list, TimeEntry create/list) — medium (3–5 days)
5. Unit tests for ViewModels and models — small (2 days)
6. Integration / manual tests & quickstart README, sample Firebase rules — small (1–2 days)

## Acceptance / Verification Plan

Automated tests
- Unit tests: service mocks (done) + viewmodel tests ensuring flows call IFirebaseDatabaseService correctly (to be added)
- Integration tests (smoke): run against either Firebase Emulator Suite or a test Firebase project using environment variables (FIREBASE_DATABASE_URL) — test both success and permission denied flows

Manual checks
- Create a test account, create client/invoice/time entry records via app, and confirm they land under /users/{uid}/ in the DB using Firebase console
- Attempt cross-user access and confirm server rules block access

Success criteria
- All unit tests pass and meet coverage targets for CRUD flows  
- Integration/manual tests confirm data is scoped to users and server rules prevent cross-user access  
- Developer experience: clear README and quickstart; environment variables documented (FIREBASE_DATABASE_URL) and helpful startup diagnostics

## Risks & Mitigations

- Risk: Token refresh absent → long-running sessions may fail. Mitigation: document limitation and add token refresh in a subsequent task.
- Risk: Security rules misconfigured → accidental data exposure. Mitigation: include example rules and encourage testing with emulator; require manual review before production rollout.
- Risk: Offline requirements underestimated → scope creep. Mitigation: explicitly mark offline sync out of scope, create separate feature for offline-first design.

## Next Steps (this sprint)

1. Create `data-model.md` and `quickstart.md` in the spec folder (Phase 1)  
2. Add model classes and refine `FirebaseRealtimeDatabaseService` (Phase 2)  
3. Add viewmodel wiring + unit tests, then run tests and push changes  
