# Feature Specification: Firebase Realtime Database (per-user data)

**Feature Branch**: `002-firebase-realtime-db`  
**Created**: December 2, 2025  
**Status**: Draft  
**Input**: User description: "Add Firebase Realtime Database so each authenticated user can read/write/update/delete only their own data. Provide a DI-friendly service, tests, sample security rules, and UI wiring for Clients, Invoices, and Time Entries."

## Clarifications

### Questions / decisions (Dec 2025-12-02)

-- Q1: Should the client-side service support background/offline sync (queuing writes while offline) or strictly REST-based realtime operations?  
  A: REST-only v1 (no offline sync). Offline sync is explicitly out-of-scope for this iteration; we'll implement REST-backed CRUD with clean interfaces and add offline support later if requested.

-- Q2: Which logical collections should be scoped under each user by default? (Candidates: clients, invoices, timeEntries, userSettings)  
  A: Use the default set: clients, invoices, timeEntries, settings. Attachments and audit logs may be added later as separate features.

> Note: Keep clarifications limited to high-impact decisions; defaults above are reasonable and can be updated later.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Manage personal data (Priority: P1)

As an authenticated user I want to create, read, update and delete my own Clients, Invoices and Time Entries so that I can track my work and bill clients.

Why this priority: Core app functionality depends on per-user persistence of domain data.

Independent Test: Using a test account, call the REST-backed client methods to create a Client, verify it appears under /users/{uid}/clients, update a field, delete it, and confirm it no longer exists. All actions must succeed when authenticated and fail when using another user's token.

Acceptance Scenarios:

1. Given I am authenticated, When I create a resource (Client / Invoice / TimeEntry), Then the service returns success and the resource appears under my user node in the DB.
2. Given I am authenticated, When I fetch my collection, Then I receive only resources scoped to my user id.
3. Given I own a resource, When I update it, Then the changes are persisted and visible to my account on next read.
4. Given I own a resource, When I delete it, Then it is removed for my account and no other user can access it.

---

### User Story 2 - Prevent cross-user access (Priority: P1)

As a product owner I need a guarantee that users cannot read or modify other users' data so that privacy and data isolation are preserved.

Why this priority: Security and privacy are non-negotiable and must be enforced server-side.

Independent Test: Attempt accessing /users/{otherUid}/clients using the authenticated user's ID token; expect HTTP 401/403 or an empty result in accordance with configured Firebase security rules.

Acceptance Scenarios:

1. Given a valid authenticated user and token, When they request a different user's resource, Then the database rejects the request.
2. Given an unauthenticated request, When any DB endpoint is called, Then the request is rejected.

---

### User Story 3 - App session persistence & multi-device consistency (Priority: P2)

As a user I want my data to be persisted and visible when I use the app from different devices so I can continue work seamlessly.

Why this priority: Good UX — users expect their stored data to follow them across devices.

Independent Test: Create data using device A, then authenticate with same account on device B and verify all data is present and consistent.

Acceptance Scenarios:

1. Given data saved on device A, When the same user logs in on device B, Then data is available and consistent across devices.

---

### Edge Cases

- Attempt to create resources with invalid/malformed payloads — service should return validation errors (400) rather than crash.
- Token expired during operation — client should observe 401/403 and surface a friendly message asking the user to re-authenticate; token refresh is out-of-scope for v1 but must be documented.
- Race conditions on concurrent updates — document last-writer-wins semantics for v1 and plan stronger conflict handling later if needed.
- Missing FIREBASE_DATABASE_URL or invalid endpoint — client library must gracefully report configuration errors and fail fast in startup diagnostics.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The app MUST persist per-user application data (clients, invoices, time entries, user settings) to Firebase Realtime Database under a per-user namespace (e.g., /users/{uid}/...).
- **FR-002**: The client library MUST authenticate requests using the authenticated user's Firebase ID token exposed by the existing IAuthService (e.g., GetIdTokenAsync()).
- **FR-003**: The client MUST allow CRUD operations on resources scoped to the authenticated user: GetAsync, SetAsync (overwrite), PushAsync (append/collection create), DeleteAsync.
- **FR-004**: The client MUST validate basic payloads before sending (e.g., required fields, types) and return informative errors for invalid inputs.
- **FR-005**: The client MUST handle HTTP error responses (4xx/5xx) gracefully and surface actionable error messages to UI layers.
- **FR-006**: The system MUST reject requests from unauthenticated clients (server-side enforcement via Firebase Security Rules) and client code should detect and report authentication failures.
- **FR-007**: The client MUST include unit tests that mock HttpClient and IAuthService to cover successful CRUD flows and expected failures (unauthenticated, permission denied, malformed payload).
- **FR-008**: Configuration MUST be driven by environment variables for development & CI (FIREBASE_DATABASE_URL) and initialization should log helpful diagnostic messages when missing.
- **FR-009**: The first iteration MAY skip offline-write/queueing and real bi-directional synchronization; if offline support is required it will be scoped as a follow-up.

### Key Entities *(include if feature involves data)*

- **Client**: Represents a customer of the user; attributes: id (push key), name, contactEmail, notes, createdAt, updatedAt
- **Invoice**: Represents a billable document; attributes: id, clientId, amount, currency, dateIssued, dueDate, status, items[], createdAt, updatedAt
- **TimeEntry**: Work entries for tracking time; attributes: id, clientId, description, durationMinutes, startTime, endTime, tags[], createdAt, updatedAt
- **UserSettings**: Per-user preferences; attributes: timezone, currency, defaultRate, notificationPreferences

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Authenticated users can perform CRUD operations on their own resources successfully in 100% of tested cases across unit tests and manual integration tests.
- **SC-002**: Unauthorized attempts to access another user's data are blocked by server rules; unit tests simulate this and expect HTTP 401/403 responses.
- **SC-003**: The client library has 100% code coverage for all positive and negative CRUD flows in unit tests (mocked HTTP/ID tokens).
- **SC-004**: App reports configuration errors at startup (missing FIREBASE_DATABASE_URL) instead of failing silently — documented and verified in integration smoke tests.

## Assumptions

- The project will use Firebase Realtime Database (not Firestore) per the user's choice.
- `IAuthService` returns a valid Firebase ID token via `GetIdTokenAsync()` for authenticated users; token refresh is out-of-scope for this initial feature.
- Server-side security rules will be authored in Firebase console to enforce per-user access; the spec will include suggested rules but not deploy them programmatically.
- Offline-first or background sync is not part of the initial milestone and can be added in a follow-up if requested.
