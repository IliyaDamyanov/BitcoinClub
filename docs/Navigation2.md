# Navigation

## Purpose

This document describes where navigation is rendered in the site.

## How it works

- The landing page (`BitcoinClub/Views/Home/Index.cshtml`) renders the primary navigation bar.
  - It includes the section links (home, calendar, info, links).
  - It includes the language switch button.
  - It includes authentication links via the `_LoginPartial`.
- The shared layout `BitcoinClub/Views/Shared/_Layout.cshtml` no longer renders a global top navigation bar.

## How to use / modify

- To change the landing page navigation items or layout, edit `BitcoinClub/Views/Home/Index.cshtml`.
- Authentication link contents (login/register vs manage/logout) are controlled in `BitcoinClub/Views/Shared/_LoginPartial.cshtml`.

## Architectural notes

- The landing page owns its navigation because it has a custom layout/hero design.
- Other pages can add their own navigation later (or a shared navbar can be reintroduced if needed).
