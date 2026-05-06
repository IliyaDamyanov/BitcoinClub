# Glow Pay setup

Bitcoin Club uses Glow Pay for membership/support Bitcoin payments.
The implementation is intentionally credential-free in git.

## Dashboard values

Create or open the merchant account at:

- https://glow-pay.co/dashboard

Configure these runtime settings outside source control:

| Setting | Value |
|---|---|
| `GlowPay:BaseUrl` | `https://glow-pay.co` unless Glow Pay provides another API host |
| `GlowPay:ApiKey` | Dashboard API key |
| `GlowPay:WebhookSecret` | Generated after setting a webhook URL |

## API behavior implemented

- `POST /api/payments`
  - Header: `X-API-Key`
  - Body: `amountSats`, `description`, `metadata`
  - Expected response data: `paymentId`, `paymentUrl`, `invoice`, `expiresAt`
- `GET /api/payments/{paymentId}`
  - Used by the existing manual “I have paid - verify” button
  - Treats `status = completed` as paid

## Webhook setup still required

The public dashboard bundle documents these webhook events:

- `payment.created`
- `payment.completed`
- `payment.expired`

Webhook payloads are signed with `X-Glow-Signature` using HMAC-SHA256 over the raw request body and `GlowPay:WebhookSecret`.

Current app behavior does not consume Glow Pay webhooks yet.
Set the webhook URL only after adding a raw-body webhook endpoint, or continue using manual verification.

## No secrets in git

Do not commit real dashboard values.
Use environment variables, user-secrets, or deployment secrets for live credentials.
