# Admin Panel

## Purpose
The Admin panel provides a dedicated UI surface for administrative features (management views, reports, moderation, etc.) without mixing them into the main site layout.

The Admin area is restricted to users in the `admin` role.

## How it works

### Routing
The application uses MVC Areas.

The Admin area is reachable under:
- `/Admin` (defaults to `HomeController.Index`)
- `/Admin/{controller}/{action}`

Routing is enabled via an area route in `Program.cs`:
- `{area:exists}/{controller=Home}/{action=Index}/{id?}`

### Authorization
All Admin controllers must be protected with:
- `[Authorize(Roles = "admin")]`

Role names are centralized in:
- `BitcoinClub.Infrastructure.Auth.RoleNames`

Expected behavior:
- unauthenticated users: redirected to login (challenge)
- authenticated non-admin users: `403 Forbidden`
- authenticated admin users: `200 OK`

### Layout
Admin pages use a dedicated layout:
- `Areas/Admin/Views/Shared/_AdminLayout.cshtml`

Admin views set the area layout via:
- `Areas/Admin/Views/_ViewStart.cshtml`

### Navigation menu
The admin layout provides a basic navbar with:
- Dashboard (`/Admin`)
- Back to site (`/`)

## How to use
- Add new admin controllers under `Areas/Admin/Controllers`.
- Add corresponding views under `Areas/Admin/Views/{ControllerName}`.
- Admin pages automatically use `_AdminLayout`.

## Architectural decisions
- Admin UI is implemented as an MVC Area to keep routing and views clearly separated.
- A dedicated layout minimizes the risk of mixing admin navigation with public navigation.
- Role-based authorization is applied at the controller level to ensure all actions are protected by default.
