# Auth UI customization (Register page)

## Purpose
Provide a custom registration page UI (smaller fields, centered layout, clearer labels) while continuing to use ASP.NET Identity for the underlying registration flow.

## How it works
- A custom MVC endpoint is added:
  - `GET /Account/Register`
  - `POST /Account/Register`
- The UI is implemented as an MVC view:
  - `BitcoinClub/Views/Account/Register.cshtml`
- The controller uses Identity services (`UserManager<IdentityUser>`, `SignInManager<IdentityUser>`, `IEmailSender`) to create the user and trigger the standard email confirmation flow.
- The navbar register link is updated to point to this MVC endpoint.

## Localization (BG/EN)
The Register view uses view localization (`IViewLocalizer`) so it automatically renders in the current request UI culture.

Resource files for the view:
- `BitcoinClub/Resources/Views/Account/Register.en.resx`
- `BitcoinClub/Resources/Views/Account/Register.bg.resx`

The culture is set by the existing request localization configuration in `Program.cs`.

## How to use
- Navigate to `/Account/Register`.
- Switch language using the same mechanism used on the landing page (culture cookie set by the app).

## Styling
- Auth page styling is added in `wwwroot/css/site.css` under the `/* Auth pages (custom MVC auth views) */` section.
- The styling is scoped by `.bc-auth-*` classes to avoid impacting other pages.

## Architectural decisions
- Uses an MVC controller + view (to match the rest of the app) instead of Identity UI scaffolding.
- Keeps Identity login/logout and account management pages unchanged.
- Localizes only the custom MVC view (the built-in Identity UI pages remain in their default language unless scaffolded and localized separately).
