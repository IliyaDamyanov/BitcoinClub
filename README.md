# Bitcoin Club Sofia — Membership Website

Membership management platform for [Bitcoin Club Sofia](https://www.bitcoinclub.bg/) (NGO). Built with ASP.NET Core 9.0.

## Features

- **Member Authentication** — Register, login, role-based access (Admin/Member)
- **Membership Dashboard** — View subscription status, expiry, payment history
- **Lightning Payments** — Pay membership dues via Lightning Network
- **Admin Panel** — Manage posts, content, and members
- **Google Sheets Import** — Migrate existing member data from spreadsheets
- **Localization** — Bulgarian (default) and English
- **Structured Logging** — Serilog with console + rolling file output

## Tech Stack

- .NET 9.0 / ASP.NET Core MVC
- PostgreSQL + Entity Framework Core
- ASP.NET Core Identity
- Serilog
- xUnit + Moq (88 tests)

## Getting Started

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run tests
dotnet test

# Run (requires PostgreSQL — configure connection string in appsettings.json)
dotnet run --project BitcoinClub
```

## Documentation

See the [`docs/`](docs/) folder:
- [Project Overview](docs/ProjectOverview.md)
- [Authentication](docs/Auth.md)
- [Auth Providers](docs/AuthProviders.md)
- [Database](docs/Database.md)
- [Payments](docs/Payments.md)
- [Membership / Subscriptions](docs/Subscriptions.md)
- [Landing Page](docs/LandingPage.md)
- [Localization](docs/Localization.md)
- [Logging](docs/Logging.md)
- [Admin Panel](docs/AdminPanel.md)
- [Dark Theme](docs/DarkTheme.md)

## Project Structure

```
BitcoinClub/          — Main web application
BitcoinClub.Tests/    — Test project (xUnit)
BitcoinClub.Import/   — Google Sheets CSV import CLI tool
docs/                 — Architecture documentation
```

## License

Private repository — Bitcoin Club Sofia.
