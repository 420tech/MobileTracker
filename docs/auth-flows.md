# Authentication & Registration Flows (Firebase)

This document describes the user flows for authentication and registration in the MobileTracker app, using Firebase Authentication. All diagrams are in Mermaid format for easy inclusion in documentation.

---

## 1. Registration Flow

```mermaid
flowchart TD
    A[App Launch] --> B[Login/Register Page]
    B --> C[User selects Register]
    C --> D[RegistrationPage]
    D --> E{Valid Email/Password?}
    E -- No --> F[Show Validation Error]
    E -- Yes --> G[Tap Register]
    G --> H[Show Loading]
    H --> I[Firebase Create User]
    I -- Success --> J[Go to Dashboard]
    I -- Error --> K[Show Error]
```

---

## 2. Login Flow

```mermaid
flowchart TD
    A[App Launch] --> B[Login/Register Page]
    B --> C{Valid Email/Password?}
    C -- No --> D[Show Validation Error]
    C -- Yes --> E[Tap Login]
    E --> F[Show Loading]
    F --> G[Firebase Sign-In]
    G -- Success --> H[Go to Dashboard]
    G -- Error --> I[Show Error]
```

---

## 3. Password Reset Flow

```mermaid
flowchart TD
    A[LoginPage] --> B[Tap Forgot Password?]
    B --> C{Valid Email?}
    C -- No --> D[Show Validation Error]
    C -- Yes --> E[Tap Send Reset Link]
    E --> F[Show Loading]
    F --> G[Firebase Send Reset Email]
    G -- Success --> H[Show Success]
    G -- Error --> I[Show Error]
```

---

## 4. Logout Flow

```mermaid
flowchart TD
    A[Dashboard/Menu] --> B[Tap Logout]
    B --> C[Firebase Sign-Out]
    C --> D[Clear Session]
    D --> E[Go to Login/Register Page]
```

---

## 5. Session Persistence

```mermaid
flowchart TD
    A[App Launch] --> B[Check Firebase Auth State]
    B -- Authenticated --> C[Go to Dashboard]
    B -- Not Authenticated --> D[Show Login/Register]
```

---

> Copy these diagrams into your README or documentation for clear, visual user flow representation.
