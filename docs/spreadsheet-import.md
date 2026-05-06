# Spreadsheet Import

Use `BitcoinClub.Import` for one-shot or repeatable imports from the current Google Sheets CSV export into the dynamic PostgreSQL database.

## Input

Export the spreadsheet as CSV and keep the same column layout as `TestData/INCOMES AND EXPENSES.csv`:

- name, Discord nickname, email, position
- total contribution text
- member-since year
- monthly payment columns such as `Месечна вноска 9/2024`, `10/2024`, `11/2024`
- volunteer interests, phone, address, secondary email, notes

The importer intentionally prints aggregate counts only. Do not paste private spreadsheet rows into logs or docs.

## Database mapping

- `AspNetUsers`
  - matched by normalized email when present
  - fallback imported email is generated from Discord nickname or name under `@import.bitcoinclub.local`
  - phone is updated when present
- `ImportedMemberProfiles`
  - stores spreadsheet-only member metadata: full name, Discord nickname, position, member-since date, total contributions text, interests, address fields, secondary email, and notes
- `Subscriptions`
  - one subscription per user
  - expiration date is the last paid/free month plus one month
  - last payment date is the latest paid/free month
- `Payments`
  - one completed/free spreadsheet payment per non-empty monthly cell
  - provider is `spreadsheet`
  - provider payment id is deterministic, so reruns do not duplicate payment history
  - `AmountSats` is `0` because the spreadsheet stores BGN/free entries, not Lightning sats

## Run

From the repository root:

```bash
ConnectionStrings__DefaultConnection='Host=...;Database=...;Username=...;Password=...' \
  dotnet run --project BitcoinClub.Import -- "path/to/export.csv"
```

For a safe summary without saving changes:

```bash
ConnectionStrings__DefaultConnection='Host=...;Database=...;Username=...;Password=...' \
  dotnet run --project BitcoinClub.Import -- "path/to/export.csv" --dry-run
```

The command runs EF migrations before import, then prints:

- dry-run flag
- rows processed / skipped
- users, profiles, subscriptions, and payments upserted

## Safety

- Reruns are idempotent for users, subscriptions, member profiles, and payment history.
- Existing users are not destructively overwritten; only phone is filled/updated from the spreadsheet.
- Payment history is append-only by deterministic provider id.
- Ambiguous rows with no name, email, or Discord nickname are skipped.
