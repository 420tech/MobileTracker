# Data Model: Firebase Authorization Services

**Date**: December 1, 2025  
**Feature**: Firebase Authorization Services  

## Entities

### User Account

**Description**: Represents a registered user in the Firebase Authentication system.

**Fields**:
- `Email`: string (required, unique) - User's email address
- `RegistrationDate`: DateTime (required) - When account was created
- `LastLoginDate`: DateTime (optional) - Last successful login timestamp
- `AccountStatus`: enum {Active, Locked} (required) - Current account state

**Validation Rules**:
- Email: Must be RFC 5322 compliant format
- Password: Minimum 8 characters with at least one letter and one number (stored securely in Firebase)
- RegistrationDate: Set automatically on account creation
- LastLoginDate: Updated on successful login
- AccountStatus: Defaults to Active, set to Locked after 5 failed login attempts

**State Transitions**:
- Unregistered → Registered: Account creation with valid email/password
- Registered → Logged In: Successful authentication
- Logged In → Logged Out: User logout or session expiration
- Active → Locked: After 5 consecutive failed login attempts (15 minute lockout)
- Locked → Active: After 15 minutes or manual unlock

**Relationships**:
- None (standalone authentication entity)

**Notes**:
- Password is managed by Firebase and not stored locally
- Account lockout is implemented using Firebase custom claims
- All timestamps use UTC