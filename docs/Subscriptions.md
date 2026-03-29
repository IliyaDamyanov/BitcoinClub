# Subscriptions

## What a subscription is
A subscription represents a member�s access period to the Bitcoin Club. It records when access expires and tracks the last successful payment.

## How it is stored
Subscriptions are stored in PostgreSQL using EF Core in the `Subscriptions` table.

Each subscription belongs to an ASP.NET Identity user:

- `Subscription.UserId` is a foreign key to `AspNetUsers.Id`

Fields:
- `Id` (GUID)
- `UserId` (string)
- `ExpirationDate` (DateTime)
- `LastPaymentDate` (DateTime?)
- `CreatedAt` (DateTime)

## Dashboard
The member dashboard is available at:

- `GET /Membership`

Behavior:
- Requires authentication
- Loads the logged-in user�s `Subscription`
- Displays subscription dates and a placeholder for payment history

## How it will be used later
Later tasks will extend subscriptions to support:

- Lightning subscription payments via LNbits (already implemented)
- Extending `ExpirationDate` after successful payments (already implemented)
- Access control that checks subscription status before allowing member-only actions
- Admin views for managing/inspecting subscriptions
