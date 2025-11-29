

<!--
Sync Impact Report (2025-11-29)
--------------------------------
Version change: 1.0.0 → 1.1.0
List of modified principles: None
Added sections: Git Practices, Documentation Requirements
Removed sections: None
Templates requiring updates: None (plan, spec, tasks templates already align) ✅
Follow-up TODOs: None
-->

# MobileTracker Constitution


## Core Principles

### I. Modular Architecture
All code MUST be organized into clean, modular components. Each feature or service MUST be independently testable and reusable. Cross-feature dependencies MUST be minimized. Rationale: Ensures maintainability, scalability, and clear separation of concerns.

### II. MVVM Pattern (CommunityToolkit.MVVM)
All UI logic MUST be separated from business logic using the MVVM pattern, leveraging CommunityToolkit.MVVM. ViewModels MUST contain all state and logic; Views MUST be as thin as possible. Rationale: Promotes testability, maintainability, and clear UI/business separation.

### III. Test-First Development (NON-NEGOTIABLE)
All new features and bug fixes MUST be developed using a test-first approach. Automated unit and integration tests MUST be written before implementation. Red-Green-Refactor cycle is strictly enforced. Rationale: Ensures reliability and prevents regressions.

### IV. CommunityToolkit.MAUI Usage
All cross-platform UI and utility features MUST use CommunityToolkit.MAUI where applicable. Custom controls or behaviors MUST only be created if toolkit features are insufficient. Rationale: Ensures consistency, leverages community best practices, and reduces maintenance burden.

### V. Code Quality & Observability
All code MUST follow .NET 10 and MAUI best practices, including async/await, dependency injection, and structured logging. Observability (logging, error reporting) MUST be implemented for all critical paths. Rationale: Ensures code is robust, debuggable, and production-ready.


## Technology Stack & Compliance

- The project MUST use .NET 10, MAUI, CommunityToolkit.MAUI, and CommunityToolkit.MVVM as primary frameworks.
- All dependencies MUST be open source or have approved licenses.
- All code MUST be cross-platform (Android, iOS, Windows at minimum).
- Security best practices (input validation, secure storage, least privilege) MUST be followed.
- Performance standards: App startup < 2s, UI response < 100ms, memory usage < 150MB on target devices.



## Development Workflow

- All work MUST be tracked via feature branches and pull requests.
- Code reviews are MANDATORY for all merges to main.
- All PRs MUST pass automated tests and linters before merge.
- Each feature/release MUST include updated documentation and tests.
- Breaking changes MUST be versioned and documented per semantic versioning.

## Git Practices

- The `main` branch MUST always be deployable and stable.
- All feature work MUST occur in topic branches named `feature/<short-description>`.
- Bugfixes MUST use `fix/<short-description>`; hotfixes use `hotfix/<short-description>`.
- Pull requests MUST reference related issues and include a clear summary.
- Commits MUST be atomic, with clear, conventional messages (e.g., `feat:`, `fix:`, `docs:`).
- Squash merging is preferred unless a linear history is required.

## Documentation Requirements

- All public APIs, services, and components MUST have XML or Markdown documentation.
- Each feature/release MUST update the user-facing documentation (e.g., README, quickstart, or in-app help).
- All code examples MUST be tested and up to date.
- Architectural decisions and rationale MUST be documented in an ADR (Architecture Decision Record) or similar file.
- Onboarding documentation MUST be maintained and reviewed quarterly.


## Governance

- This constitution supersedes all other development practices and policies.
- Amendments require a documented proposal, team approval, and a migration plan for compliance.
- All PRs and reviews MUST verify compliance with the constitution.
- Any complexity or deviation MUST be justified and documented in the implementation plan.

**Version**: 1.1.0 | **Ratified**: 2025-11-29 | **Last Amended**: 2025-11-29
<!-- Version: 1.1.0 | Ratified: 2025-11-29 | Last Amended: 2025-11-29 -->
