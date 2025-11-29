# MobileTracker Project Checklist

## Phase 1: Setup
- [x] Scaffold .NET 10 MAUI project
- [x] Add CommunityToolkit.MAUI and CommunityToolkit.MVVM
- [x] Resolve any package version issues

## Phase 2: Foundational
- [x] Create XAML and code-behind for:
  - [x] Dashboard page
  - [x] TimeTracker page
  - [x] Client page
  - [x] Invoice page
  - [x] Settings page
- [x] Create ViewModels for each page using CommunityToolkit.MVVM

## Phase 3: Configure AppShell Navigation
- [x] Update AppShell.xaml for tabs (mobile) and toolbars (desktop)
- [x] Register navigation routes for all pages in AppShell.xaml.cs

## Phase 4: Register DI for Views and ViewModels
- [x] Register all ViewModels and Views for dependency injection in MauiProgram.cs
- [x] Add CommunityToolkit.Maui initialization

## Phase 5: Implement Basic UI and Navigation
- [ ] Add basic UI elements to each page
- [ ] Ensure navigation works for all pages
- [ ] Test navigation on mobile and desktop platforms

## Phase 6: Polish & Documentation
- [ ] Add XML/Markdown documentation for all public APIs, services, and components
- [ ] Update README/quickstart and onboarding docs
- [ ] Document architectural decisions (ADRs)
