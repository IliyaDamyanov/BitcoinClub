# Landing page (Home)

## Purpose
Provide a single-page landing experience for the Bitcoin Club website (navbar + anchored sections) with a BG/EN language toggle.

## How it works
- `HomeController.Index` reads an optional `lang` query string (`BG` or `EN`) and maps it to a request culture (`bg` / `en`).
- Landing page UI labels (navbar items, section titles, etc.) are stored in shared resource files:
  - `BitcoinClub/Resources/LandingPageStrings.resx` (BG/default)
  - `BitcoinClub/Resources/LandingPageStrings.en.resx` (EN)
- `LandingPageContentService` uses `IStringLocalizer<LandingPageStrings>` to read the localized strings and populate the `LandingPageViewModel`.
- `Views/Home/Index.cshtml` renders:
  - a dark Bootstrap navbar with anchor links (`#home`, `#calendar`, `#info`, `#links`)
  - a hero section with an image and goals list
  - a “means” section
  - a Google Calendar iframe for events
  - association info, social links, membership/support, contacts
  - useful links
- `wwwroot/css/landing.css` provides the minimal styling for the language button.

## How to use
- Open `/` for BG.
- Open `/?lang=EN` for English.
- Use the language button in the navbar to toggle.

## Architectural decisions
- Kept the existing query-string language toggle, but mapped it to cultures so `.resx` resources can be reused across pages.
- Localizable UI labels are moved to resource files to enable reuse on other pages.
