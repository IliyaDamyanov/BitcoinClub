# Authentication

## Purpose
This project uses ASP.NET Core Identity (Individual Accounts) for email/password authentication.

## Identity setup
- Identity is configured in `Program.cs` using `AddDefaultIdentity<IdentityUser>()` with EF Core stores backed by `ApplicationDbContext`.
- Authentication cookies are enabled via `UseAuthentication()` and `UseAuthorization()`.
- Unique email addresses are enforced using `options.User.RequireUniqueEmail = true`.

## Registration / login / logout flow
Identity UI provides Razor Pages endpoints under the `Identity` area:

- Register: `/Identity/Account/Register`
- Login: `/Identity/Account/Login`
- Logout (POST): `/Identity/Account/Logout`

`Views/Shared/_LoginPartial.cshtml` renders links for login/register or logout/manage depending on the current authentication state.

## Roles
### Roles
The application uses two roles:

- `member`
- `admin`

Role name constants are defined in `BitcoinClub.Infrastructure.Auth.RoleNames`.

### Role seeding
On application startup, roles are ensured to exist via `BitcoinClub.Infrastructure.Auth.RoleSeeder`.

This requires Identity role services to be enabled (configured with `.AddRoles<IdentityRole>()`).

### Default role assignment
New users are assigned the `member` role by default during claims principal creation.

This is implemented by `BitcoinClub.Infrastructure.Auth.DefaultRoleUserClaimsPrincipalFactory`.

## Protected pages
Example protected endpoint:

- `GET /Members` requires the `member` role.

## Database
Identity tables are created via EF Core migrations in PostgreSQL.

- Ensure the database exists and migrations are applied:
  - `dotnet ef database update --project BitcoinClub --startup-project BitcoinClub`
