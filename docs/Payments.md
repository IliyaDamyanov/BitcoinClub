# Payments

## Purpose
This project uses the Breez SDK to support Lightning-based subscription payments.

This task adds an application-level payment service (`IBreezePaymentService`) that:
- initiates a membership payment (creates a Lightning invoice)
- verifies a payment and updates the `Subscription` row in the database

## Configuration
Configuration is read from the `Breez` section.

Keys:
- `Breez:ApiKey` (required)
- `Breez:Environment` (optional, default `test`)
- `Breez:DefaultNodeUrl` (optional)

## How it works

### Initiation
`IBreezePaymentService.InitiateMembershipPaymentAsync(...)` performs:
1. basic validation (`userId`, `amountSats`)
2. calls `IBreezApiClient.CreateInvoiceAsync(...)` to create an invoice
3. ensures a `Subscription` row exists for the user (creates one if missing)
4. returns a `PaymentInitiationResult` containing:
   - `SubscriptionId` (used later to apply updates)
   - `PaymentId` (provider identifier)
   - optional `PaymentRequest` (BOLT11)

### Verification
`IBreezePaymentService.VerifyPaymentAsync(...)` performs:
1. calls `IBreezApiClient.GetPaymentStatusAsync(paymentId)`
2. if not paid: returns `IsPaid = false` and does NOT change the DB
3. if paid:
   - updates `Subscription.LastPaymentDate`
   - extends `Subscription.ExpirationDate` by 1 month (from now or existing expiration, whichever is later)

## Usage
Register in `Program.cs` (already provided via `AddBreezPayments`):
- `IBreezePaymentService` is `Scoped`
- `IBreezApiClient` is `Singleton`

In a controller/page:
- call `InitiateMembershipPaymentAsync(...)` to get `PaymentRequest` and show it to the user
- later call `VerifyPaymentAsync(subscriptionId, paymentId)` to apply the renewal

## Architectural decisions
- A small abstraction (`IBreezApiClient`) is introduced so payment logic can be unit tested without requiring the Breez SDK or network calls.
- A placeholder `BreezApiClient` is registered; it is intentionally minimal and should be replaced with a real Breez SDK-backed implementation.

## Security considerations
- Do not commit real API keys to source control.
- Prefer using environment variables, user secrets, or a deployment secret store.
- Treat any Breez credentials and node configuration as sensitive.
- Avoid logging configuration values.
