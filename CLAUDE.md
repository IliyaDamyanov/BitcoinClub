# CLAUDE.md — BitcoinClub Project

## Overview
Membership website for Bitcoin Club Sofia (NGO). ASP.NET Core MVC with PostgreSQL, Identity auth, Lightning payments, and member subscription management.

**Live site (current):** https://www.bitcoinclub.bg/ (static Next.js page by Alexander — this project replaces it)

## Quick Start

```bash
# Prerequisites: .NET 9 SDK
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools

# Restore and build
dotnet restore
dotnet build

# Run tests
dotnet test

# Run the app (needs PostgreSQL — see Database section)
dotnet run --project BitcoinClub
```

## Project Structure

```
BitcoinClub/                    # Main web application
├── Areas/
│   ├── Admin/                  # Admin panel (posts, dashboard)
│   └── Identity/               # ASP.NET Identity pages
├── Controllers/
│   ├── AccountController.cs    # Registration, login
│   ├── HomeController.cs       # Landing page
│   ├── MembershipController.cs # Member dashboard
│   └── PaymentsController.cs   # Lightning payment flow
├── Data/
│   ├── ApplicationDbContext.cs  # EF Core context
│   └── Migrations/             # EF Core migrations (PostgreSQL)
├── Infrastructure/
│   ├── Auth/                   # Roles, claims, auth providers
│   ├── Database/               # Connection string validation
│   ├── Files/                  # Image upload service
│   ├── Payments/               # Lightning payment service (LNbits)
│   └── Social/                 # Social media publishers (STUBS — scheduled for removal)
├── Models/                     # Domain models (Post, Payment, Subscription)
├── Resources/                  # Localization (bg/en)
├── Services/                   # Business logic services
├── ViewModels/                 # View models
├── Views/                      # Razor views
└── wwwroot/                    # Static assets

BitcoinClub.Tests/              # xUnit test project (88 tests)
├── Admin/                      # Admin panel tests
├── Auth/                       # Registration, login, roles tests
├── AuthProviders/              # Auth provider contract tests
├── Database/                   # DB config and migration tests
├── Files/                      # File upload tests
├── Integration/                # End-to-end integration tests
├── Landing/                    # Landing page tests
├── Localization/               # i18n tests
├── Membership/                 # Membership dashboard tests
├── Payments/                   # Payment flow tests
├── Services/                   # Service layer tests
├── SocialMedia/                # Social media tests (to be removed)
├── StaticAssets/               # CSS/theme tests
├── Subscriptions/              # Subscription model tests
└── Views/                      # View rendering tests

BitcoinClub.Import/             # CLI tool: import members from Google Sheets CSV
docs/                           # Architecture documentation
```

## Architecture Decisions

- **ASP.NET Core MVC** with Razor views (not SPA — appropriate for club website)
- **PostgreSQL** via Npgsql + EF Core (production), InMemory provider (tests)
- **ASP.NET Core Identity** for auth with role-based access (Admin, Member)
- **Serilog** for structured logging (console + rolling file in `logs/`)
- **Localization** supports Bulgarian (default) and English
- **Lightning payments** via LNbits REST API

## Database

PostgreSQL connection string in `appsettings.json`:
```
Host=localhost;Port=5432;Database=bitcoinclubdb;Username=...;Password=...
```

Apply migrations:
```bash
dotnet ef database update --project BitcoinClub
```

## Logging

Uses Serilog with:
- **Console sink** — structured output during development
- **File sink** — rolling daily logs in `logs/` directory (30 day retention)
- HTTP request logging via `UseSerilogRequestLogging()`
- Key events logged: user registration, payment initiation/verification

## Key Conventions

- **Nullable reference types** enabled across all projects
- **Sealed classes** for controllers and services where inheritance isn't needed
- **Interface-first** design for all services (DI registered in Program.cs)
- Tests use **xUnit** + **Moq** + EF Core InMemory provider
- Integration tests use `WebApplicationFactory<Program>`

## Current Status & Known Issues

### Working
- Auth (register, login, roles, admin access control)
- Admin panel (create/list posts, image upload)
- Membership model and dashboard skeleton
- Google Sheets CSV import tool
- Subscription and payment data models
- 76/88 tests passing

### Needs Work
- Landing page needs design work
- Landing page needs design work
- No SQLite fallback for local development yet
- `async void` test methods need conversion to `async Task` (xUnit v3 migration)

### Do Not Touch
- `appsettings.json` contains real credentials — never commit changes to connection strings or API keys
- LNbits API key in appsettings is sensitive — controls wallet funds

## Testing

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~PaymentsControllerTests"

# Run with verbose output
dotnet test -v detailed
```

Some tests require PostgreSQL (integration tests marked with `Postgres` in name). Most tests use InMemory provider.

## Deployment Target

- **Railway.app** or **DigitalOcean** (PostgreSQL + .NET 9 container)
- Docker deployment planned (Dockerfile not yet created)
- Domain: bitcoinclub.bg (redirect from current static site)
