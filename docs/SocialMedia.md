# Social Media

## Purpose
The Social Media module will manage outbound posts to external platforms (Facebook, Instagram, Threads, Twitter/X, Nostr).

This task introduces a `Post` entity for storing post content and metadata before/after publishing.

## Data model

### `Post`
Fields:
- `Id` (GUID)
- `AdminUserId` (Identity user id of the admin who created the post)
- `TextContent` (post body)
- `ImagePaths` (JSON array of strings; server-side stored paths)
- `Platforms` (JSON array of strings; selected targets)
- `CreatedAt` (UTC timestamp)

## How it works
- Posts are stored in the database as rows in `Posts`.
- `ImagePaths` and `Platforms` are stored as `jsonb` in Postgres.

## How to use
- Create a `Post` object, set `AdminUserId`, `TextContent`, platform list, and image paths.
- Add it via `ApplicationDbContext.Posts` and save.

## Architectural decisions
- Platform selection is stored as strings in JSON to keep initial schema flexible while integrations are implemented.
- `AdminUserId` is a FK to Identity users; delete behavior is `Restrict` to preserve posting history.
