# Implementation Tasks: Firebase Realtime Database

This file breaks the work into discrete tasks that can be assigned, tracked and tested. Tasks are ordered roughly by dependency and priority.

T001 — Create data-model.md (P0)
- Description: Author JSON shapes for Client, Invoice, TimeEntry and UserSettings and update spec.
- Acceptance: data-model.md created with clear examples and sample validation rules.

T002 — Add domain model classes (P0)
- Description: Add `Client`, `Invoice`, `TimeEntry`, and `UserSettings` POCOs under `MobileTracker/Models` with minimal validation helpers.
- Acceptance: Classes added with unit tests validating JSON serialization shapes.

T003 — Harden FirebaseRealtimeDatabaseService (P0)
- Description: Ensure service validates inputs, maps HTTP errors to meaningful exceptions, adds null checks, and logs helpful diagnostics.
- Acceptance: Existing service passes additional unit tests for validation and error handling.

T004 — Add ViewModel CRUD flows (P1)
- Description: Implement ViewModels and small UI pages for Clients, Invoices, TimeEntries; these will call `IFirebaseDatabaseService`.
- Acceptance: Unit tests for viewmodels and manual smoke UI flows confirm end-to-end CRUD.

T005 — Integration + CI (P1)
- Description: Add integration tests (smoke) that can run against Firebase Emulator or test project and add instructions to CI to run these where feasible.
- Acceptance: Integration smoke tests run successfully in at least one environment.

T006 — Security rules examples & docs (P0)
- Description: Create recommended Firebase Realtime Database rules included in `specs/002-firebase-realtime-db/quickstart.md` demonstrating per-user access enforcement.
- Acceptance: Rules are documented and validated against emulator with example requests.

T007 — Documentation + Quickstart (P0)
- Description: Quickstart to set FIREBASE_DATABASE_URL, run tests against emulator and add developer notes (e.g., token refresh caveat).
- Acceptance: README and quickstart docs present and verified.

T008 — Optional follow-up: Offline queueing / Realtime listeners (P2)
- Description: If requested, scope and implement offline-first behavior including queue, retry, and delta-sync listeners.
- Acceptance: New feature spec and tests for offline behavior.
