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

## Protected pages
Example protected endpoint:

- `GET /Members` requires an authenticated user (controller protected with `[Authorize]`).

## Database
Identity tables are created via EF Core migrations in PostgreSQL.

- Ensure the database exists and migrations are applied:
  - `dotnet ef database update --project BitcoinClub --startup-project BitcoinClub`
