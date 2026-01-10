# Dark page background

## Purpose

Make the entire site render on a dark background (black page chrome, dark gray content area) instead of the default white Bootstrap page background.

## How it works

- The shared stylesheet `BitcoinClub/wwwroot/css/site.css` sets a baseline color scheme:
  - `html, body { background-color: #000; color: #f8f9fa; }`
  - the main content container gets a slightly lighter background to improve readability:
    - `body > .container { background-color: #212529; }`
  - link colors are adjusted for contrast
- The top navigation bar is rendered by the shared layout `BitcoinClub/Views/Shared/_Layout.cshtml` and is explicitly styled as dark:
  - `navbar-dark bg-black`

## How to use / modify

- Adjust page/background colors in `BitcoinClub/wwwroot/css/site.css`.
- Adjust navbar styling in `BitcoinClub/Views/Shared/_Layout.cshtml`.
- If you later adopt a full Bootstrap dark theme, you can replace these baseline rules with Bootstrap variables/theme overrides.

## Architectural notes

- Kept this as CSS + Bootstrap class changes so no controller/view logic changes are required and the change is applied site-wide.
