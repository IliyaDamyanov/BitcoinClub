# Public Landing Page

## Purpose
The landing page is the public entry point for the Bitcoin Club site. It explains what the club is, what it does, and how to contact the organizers.

It also embeds a public events calendar so visitors can quickly see upcoming meetups.

## How it works
- `HomeController.Index` returns a `LandingPageViewModel`.
- `Views/Home/Index.cshtml` renders the landing page using the view model values.

## Content
The page currently displays:
- Club name
- Description
- Mission
- Events calendar (embedded Google Calendar iframe)
- Contact information (email, optional Telegram)

## Google Calendar embed
- The calendar is rendered via a responsive `<iframe>` using Bootstrap’s `ratio` utility.
- The iframe URL is currently a placeholder (`calendar.google.com/calendar/embed?mode=AGENDA`).
  - Replace it with your public calendar embed URL.

## How to customize
Edit defaults in:
- `ViewModels/LandingPageViewModel.cs`

If you later want to make this content editable via the admin panel, the same fields can be moved into a database-backed settings table.

## Architectural decisions
- Content is provided by a small view model to keep the view simple and testable.
- Defaults live in code for now to avoid introducing a settings UI or configuration requirements in this task.
- The calendar embed is a simple iframe to avoid introducing API keys or OAuth flows.
