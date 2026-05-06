# Bitcoin Club — Backend Guide

> Documents project-specific backend conventions — where things live, which libraries, which patterns.
> Builder: read this before any backend work. All new patterns must be documented here.
> Update only when adding NEW patterns, not when values change.

---

## Stack

- **Framework:** ASP.NET Core MVC on .NET 9
- **Database:** Entity Framework Core with PostgreSQL in production and SQLite/test providers where configured
- **Auth:** ASP.NET Core Identity

## Payments

Location: `BitcoinClub/Infrastructure/Payments/`

Pattern:

- Controllers depend on `IPaymentService`.
- `LightningPaymentService` owns local subscription/payment business logic.
- Provider API clients implement `ILightningApiClient`.
- Runtime payment provider registration lives in a service-collection extension.

Current provider:

- Glow Pay via `GlowPayApiClient` and `AddGlowPayPayments()`.
- Config section: `GlowPay`.
- API key is sent as `X-API-Key` for payment creation.
- Manual verification checks `GET /api/payments/{paymentId}` and treats `status = completed` as paid.
- Glow Pay webhooks are handled by `Controllers/GlowPayWebhookController.cs` at `POST /webhooks/glow-pay`.
- Webhook controllers read the raw request body, validate `X-Glow-Signature` with HMAC-SHA256 and `GlowPay:WebhookSecret`, then mutate local payment state only after verification.
- Local payment completion must go through `IPaymentService.CompleteProviderPaymentAsync()` so manual verification and webhooks share the same idempotent subscription-extension path.

Credential rules:

- Never commit real API keys or webhook secrets.
- Keep only placeholder values in `appsettings.json`.
- Live values belong in environment variables, user-secrets, or deployment secret storage.

## Payment setup docs

Operator setup notes live in `docs/glow-pay-setup.md`.
