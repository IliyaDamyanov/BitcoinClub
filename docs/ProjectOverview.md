# BitcoinClub Project Overview

## Purpose
BitcoinClub is a membership website for managing member accounts and (later) paid subscriptions via Lightning. The application uses ASP.NET Core with Identity (Individual Accounts) for authentication.

## High-level architecture
- ASP.NET Core web application using MVC controllers and Razor views.
- ASP.NET Core Identity provides registration, login, and account management UI under `Areas/Identity`.
- Entity Framework Core is used for persistence via `ApplicationDbContext`.

Request flow:
1. Requests are routed through ASP.NET Core middleware.
2. MVC routes map to controllers and Razor views.
3. Identity endpoints are served by Razor Pages within `Areas/Identity`.
4. Data access goes through EF Core `DbContext`.

## Folder structure
- `Controllers/`: MVC controllers (e.g., `HomeController`).
- `Views/`: Razor views for MVC.
- `Areas/Identity/`: Identity UI implemented as Razor Pages.
- `Data/`: EF Core context and migrations.
- `Models/`: View models and simple data models.
- `wwwroot/`: Static assets.
- `docs/`: Project documentation.
