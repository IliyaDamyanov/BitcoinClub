# Social Media

## Purpose
The Social Media module manages outbound posts to external platforms (Facebook, Instagram, Threads, Twitter/X, Nostr).

It currently provides:
- a `Post` entity stored in PostgreSQL
- an Admin UI for creating and listing posts

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
- Admin UI stores uploaded images under `wwwroot/uploads/posts` and saves web paths (e.g. `/uploads/posts/<file>`).

## Admin Post Editor (MVC Area)
Routes:
- List posts: `GET /Admin/Posts`
- Create post form: `GET /Admin/Posts/Create`
- Create post submit: `POST /Admin/Posts/Create`

Create flow:
1. Admin enters `TextContent`, selects one or more platforms, and optionally uploads images.
2. Images are copied to `wwwroot/uploads/posts`.
3. A `Post` row is created with `Platforms` and `ImagePaths` populated.

Supported platforms (current UI list):
- facebook
- instagram
- threads
- twitter
- nostr

## Architectural decisions
- Platform selection is stored as strings in JSON to keep initial schema flexible while integrations are implemented.
- `AdminUserId` is a FK to Identity users; delete behavior is `Restrict` to preserve posting history.
- Uploaded images are stored on disk for now; this can be swapped for object storage later.
