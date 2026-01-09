# Social Media

## Purpose
The Social Media module manages outbound posts to external platforms (Facebook, Instagram, Threads, Twitter/X, Nostr).

It currently provides:
- a `Post` entity stored in PostgreSQL
- an Admin UI for creating and listing posts
- an image upload implementation for storing local images under `wwwroot/uploads`
- publisher interfaces to standardize how posts are published to external platforms

## Data model

### `Post`
Fields:
- `Id` (GUID)
- `AdminUserId` (Identity user id of the admin who created the post)
- `TextContent` (post body)
- `ImagePaths` (JSON array of strings; relative paths under `uploads/`)
- `Platforms` (JSON array of strings; selected targets)
- `CreatedAt` (UTC timestamp)

## How it works
- Posts are stored in the database as rows in `Posts`.
- `ImagePaths` and `Platforms` are stored as `jsonb` in Postgres.

### Image uploads
- Uploaded images are saved under: `wwwroot/uploads/posts/`
- Only relative paths are stored in the database (example: `uploads/posts/<file>.png`).
- The application can serve these via static files at: `/uploads/posts/<file>.png`.

Basic validation:
- file name must not include directory segments
- extension must be one of: `.jpg`, `.jpeg`, `.png`, `.webp`

## Publishing

### Publisher interface
Publishing is standardized through:
- `BitcoinClub.Infrastructure.Social.ISocialMediaPublisher`

Each publisher exposes:
- `Platform` (string key matching values stored in `Post.Platforms`)
- `PublishAsync(Post)` returning a `PublishResult`

### Platform publishers
The project currently contains stubs (not implemented yet):
- `FacebookPublisher` (`Platform = facebook`)
- `InstagramPublisher` (`Platform = instagram`)
- `ThreadsPublisher` (`Platform = threads`)
- `TwitterPublisher` (`Platform = twitter`)
- `NostrPublisher` (`Platform = nostr`)

Publishers are registered in DI as `ISocialMediaPublisher` so they can be discovered as an enumerable and selected by platform.

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
- Publishing uses a small interface per platform to keep integrations isolated and testable.
