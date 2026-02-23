# BitcoinClub Project Overview

## Purpose
BitcoinClub is a membership website for Bitcoin Club Sofia (NGO, EIK: 208038371). It manages member accounts, paid subscriptions via Lightning Network, and club content. Replaces the current static landing page at bitcoinclub.bg.

## High-level Architecture
- ASP.NET Core 9.0 web application using MVC controllers and Razor views
- ASP.NET Core Identity provides registration, login, role-based access, and account management
- Entity Framework Core with PostgreSQL for persistence
- Serilog for structured logging (console + rolling file)
- Localization support for Bulgarian (default) and English

## Request Flow
1. Requests are routed through ASP.NET Core middleware (Serilog request logging → static files → routing → auth)
2. MVC routes map to controllers and Razor views
3. Identity endpoints are served by Razor Pages within `Areas/Identity`
4. Admin panel is an MVC Area under `Areas/Admin` with role-based access
5. Data access goes through EF Core `DbContext`

## Folder Structure
- `Controllers/` — MVC controllers (Home, Account, Membership, Payments)
- `Views/` — Razor views for MVC
- `Areas/Admin/` — Admin panel (posts, dashboard)
- `Areas/Identity/` — Identity UI (Razor Pages)
- `Data/` — EF Core context and migrations
- `Infrastructure/` — Cross-cutting: Auth, Database, Files, Payments, Social
- `Models/` — Domain models (Post, Payment, Subscription)
- `Services/` — Business logic layer
- `ViewModels/` — View models for Razor views
- `Resources/` — Localization resource files (bg/en)
- `wwwroot/` — Static assets (CSS, JS, images, uploaded files)
- `logs/` — Serilog log files (gitignored)
- `docs/` — Project documentation
- `BitcoinClub.Tests/` — xUnit test project
- `BitcoinClub.Import/` — CLI tool for Google Sheets CSV import

## Key Technologies
| Component | Technology | Version |
|-----------|-----------|---------|
| Framework | ASP.NET Core | 9.0 |
| ORM | Entity Framework Core | 9.0.3 |
| Database | PostgreSQL | via Npgsql 9.0.4 |
| Auth | ASP.NET Core Identity | 9.0.3 |
| Logging | Serilog | 9.0.0 |
| Testing | xUnit + Moq | 2.9.3 / 4.20.72 |
| Payments | Lightning (LNbits) | In progress |
