# Firebase Authorization Quickstart

**Feature**: Firebase Authorization Services  
**Date**: December 1, 2025  

## Overview

This guide shows how to use the Firebase authentication features in the MobileTracker MAUI app.

## Prerequisites

- Firebase project configured with Authentication enabled
- FirebaseAuthentication.net NuGet package installed
- Valid Firebase configuration in the app

## User Registration

1. Navigate to the Registration page
2. Enter a valid email address (RFC 5322 compliant)
3. Enter a password (minimum 8 characters with letters and numbers)
4. Tap "Register"
5. Account created and automatically logged in

## User Login

1. Navigate to the Login page
2. Enter your registered email and password
3. Tap "Login"
4. Redirected to dashboard on success

## Password Reset

1. On the Login page, tap "Forgot Password"
2. Enter your registered email
3. Tap "Reset Password"
4. Check email for reset link (no expiration)
5. Click link and set new password

## Account Security

- Accounts are locked after 5 consecutive failed login attempts
- Lockout lasts 15 minutes
- Use specific error messages for better UX
- Secure token storage on device

## Error Handling

Common error messages:
- "Email already registered" - Email in use
- "Invalid email format" - Email doesn't meet RFC 5322
- "Incorrect password" - Wrong password entered
- "Email not found" - Account doesn't exist
- "Account locked" - Too many failed attempts

## Troubleshooting

- Ensure internet connection for authentication
- Check Firebase project configuration
- Verify email format compliance
- Wait 15 minutes if account locked