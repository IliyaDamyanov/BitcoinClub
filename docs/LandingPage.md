# Public Landing Page

## Purpose
The landing page is the public entry point for the Bitcoin Club site. It explains what the club is, what it does, and how to contact the organizers.

## How it works
- `HomeController.Index` returns a `LandingPageViewModel`.
- `Views/Home/Index.cshtml` renders the landing page using the view model values.

## Content
The page currently displays:
- Club name
- Description
- Mission
- Contact information (email, optional Telegram)

## How to customize
Edit defaults in:
- `ViewModels/LandingPageViewModel.cs`

If you later want to make this content editable via the admin panel, the same fields can be moved into a database-backed settings table.

## Architectural decisions
- Content is provided by a small view model to keep the view simple and testable.
- Defaults live in code for now to avoid introducing a settings UI or configuration requirements in this task.
