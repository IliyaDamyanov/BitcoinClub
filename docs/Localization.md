# Localization (BG/EN)

## Purpose

Provide reusable Bulgarian/English UI strings via `.resx` resource files so multiple pages can share the same translations.

## How it works

- Localization is enabled in `BitcoinClub/Program.cs` using `AddLocalization()`.
- Request culture is set via the existing `lang` query string in `HomeController.Index`:
  - `BG` -> `bg`
  - `EN` -> `en`
  - The culture is also persisted to the standard ASP.NET Core request culture cookie.
- Landing page resources live in:
  - `BitcoinClub/Resources/LandingPageStrings.resx` (default/BG)
  - `BitcoinClub/Resources/LandingPageStrings.en.resx` (EN)
- `LandingPageContentService` uses `IStringLocalizer<LandingPageStrings>` to read those keys.

## How to add resources for other pages

1. Create a marker class in `BitcoinClub/Resources` (example: `SomePageStrings.cs`).
2. Add resource files next to it:
   - `SomePageStrings.resx` (BG)
   - `SomePageStrings.en.resx` (EN)
3. Inject `IStringLocalizer<SomePageStrings>` into the service/controller that prepares your view model.

## Notes

- This keeps a single localization mechanism (resources + `IStringLocalizer`) while preserving your simple query-string toggle.
