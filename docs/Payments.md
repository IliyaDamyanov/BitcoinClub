# Payments

## Purpose
This project uses LNbits to support Lightning-based subscription payments.

The application contains:
- a payment service (`IPaymentService`) for initiating and verifying payments
- an MVC UI (`PaymentsController` + Razor views) for members to view status and create/verify invoices
- persistence of payment attempts/receipts in the `Payments` table

## Configuration
Configuration is read from the `LNbits` section in `appsettings.json`.

Keys:
- `LNbits:BaseUrl` (required, e.g. `https://legend.lnbits.com`)
- `LNbits:ApiKey` (required — Admin API key from your LNbits wallet)

## Data model

### `Subscription`
Stores membership validity:
- `ExpirationDate`
- `LastPaymentDate`

### `Payment`
Stores the payment record (provider-agnostic fields) for membership payments:
- `Provider` (currently `lnbits`)
- `ProviderPaymentId` (payment hash returned by LNbits)
- `AmountSats`
- `PaymentRequest` (BOLT11 invoice)
- `Status` (`initiated`, `pending`, `paid`)
- `PaidAt`

## How the MVC flow works

### 1) View subscription status
`GET /Payments/Status`
- loads the current user's `Subscription`
- displays expiration + last payment date

### 2) Initiate payment
`GET /Payments/Initiate`
- shows a form with `AmountSats` and `Description`

`POST /Payments/Initiate`
- calls `IPaymentService.InitiateMembershipPaymentAsync(...)`
- creates a `Payment` row with status `initiated`
- returns the same page containing the payment request (BOLT11 invoice) and a "verify" button

### 3) Verify payment
`POST /Payments/Verify`
- looks up the `Payment` row for the current user
- calls `IPaymentService.VerifyPaymentAsync(subscriptionId, providerPaymentId)`
- if unpaid: mark `Payment.Status = pending`
- if paid:
  - mark `Payment.Status = paid` and set `PaidAt`
  - subscription extension is applied by the payment service

## Architectural decisions
- The controller only orchestrates the flow and persists `Payment` rows.
- Subscription extension is performed in the service to keep the rules in one place.
- `LNbitsApiClient` implements `ILightningApiClient` — can be swapped for a self-hosted LNbits or any other Lightning backend without changing the service layer.

## Security considerations
- Do not commit real API keys to source control.
- Prefer using environment variables, user secrets, or a deployment secret store.
- Treat LNbits API keys as sensitive — they control wallet funds.
- Avoid logging configuration values.
