# Bitcoin Club — Style Guide

> Single source of truth for UI classes in this project.
> Builder: read this before any UI work. All new elements must use these classes.
> Update this file when adding NEW classes or components, not when values change.
>
> **Rules:** Follow `nexus/docs/engineering/UI_PRINCIPLES.md` — semantic class anchors first, composable modifiers, no bare utility-only elements.

---

## Stack

- **Framework:** ASP.NET Core MVC Razor views
- **Styling:** Project CSS in `BitcoinClub/wwwroot/css/`
- **Theme:** Existing Bitcoin Club theme; avoid inline styles

---

## Payment Views

`.payment-page-title` — payment page heading
`.payment-initiation-form` — Glow Pay initiation form root
`.payment-form-field` — form field wrapper
`.payment-amount-input` — sats amount input
`.payment-description-input` — payment description input
`.payment-create-button` — submit button that creates a Glow Pay invoice
`.payment-invoice-panel` — created invoice result panel
`.payment-invoice-title` — invoice panel heading
`.payment-checkout-link` — link to hosted Glow Pay checkout
`.payment-verify-form` — manual verification form
`.payment-verify-button` — manual payment verification button
