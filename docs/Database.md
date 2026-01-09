# Database

## Purpose
This project uses Entity Framework Core with PostgreSQL (via Npgsql) for:

- ASP.NET Identity schema and user data
- Application persistence through `ApplicationDbContext`

## Configuration
### Provider
The EF Core provider is `Npgsql.EntityFrameworkCore.PostgreSQL`.

### Connection string
The application reads the connection string named `DefaultConnection` from `BitcoinClub/appsettings.json`.

Format:

- `Host=localhost;Port=5432;Database=bitcoinclubdb;Username=iliyadamyanov;Password=...;`

Recommended optional settings:

- `Include Error Detail=true;` (development only)

For local development, prefer storing credentials outside source control (for example in `appsettings.Development.json` or user secrets) and keep `appsettings.json` free of real passwords.

### Program startup
At startup, `Program.cs` configures `ApplicationDbContext` using `UseNpgsql(connectionString)`.

ASP.NET Identity uses the same `ApplicationDbContext` through `AddEntityFrameworkStores<ApplicationDbContext>()`.

## Migrations
EF Core migrations are stored under `BitcoinClub/Data/Migrations`.

### Add a migration
From the repository root:

- `dotnet ef migrations add <Name> --project BitcoinClub --startup-project BitcoinClub --output-dir Data/Migrations`

### Apply migrations
From the repository root:

- `dotnet ef database update --project BitcoinClub --startup-project BitcoinClub`

If the database does not exist and your PostgreSQL user does not have permission to create databases, create it first using a superuser or an admin role, then run the update command again.

### Running database updates in WSL
If you run the app and tooling inside WSL (Ubuntu 22.04), ensure PostgreSQL is reachable from that environment.

Common setups:

- PostgreSQL runs inside WSL: use `Host=localhost;Port=5432;...`
- PostgreSQL runs on Windows: use `Host=<windows-host-ip>;Port=5432;...` (WSL cannot always reach Windows services via `localhost`)

After updating the connection string appropriately, run:

- `dotnet ef database update --project BitcoinClub --startup-project BitcoinClub`

### Notes
Migrations generated for SQL Server should not be assumed to be valid for PostgreSQL. Recreate or regenerate migrations after switching providers.
