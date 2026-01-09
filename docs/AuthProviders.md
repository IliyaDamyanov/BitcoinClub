# Auth Providers

## Purpose
The application supports multiple authentication mechanisms. To keep authentication extensible (email/password today; OAuth, LNURL-auth, Nostr login later), authentication is modeled as a provider interface.

## How it works
### Interface
`BitcoinClub.Infrastructure.Auth.Providers.IAuthProvider` defines a minimal contract:

- `Name` identifies the provider
- `SignInAsync` signs a user in
- `SignOutAsync` signs a user out

The interface is intentionally small so additional providers can be added without changing callers.

### Email/password provider
`BitcoinClub.Infrastructure.Auth.Providers.EmailPasswordAuthProvider` implements `IAuthProvider` using ASP.NET Identity's `SignInManager<IdentityUser>`:

- `SignInAsync` calls `PasswordSignInAsync` and maps the result to a provider-agnostic `AuthSignInResult`
- `SignOutAsync` calls `SignInManager.SignOutAsync`

## How to use
Providers are registered with DI. The current implementation registers `EmailPasswordAuthProvider` as an `IAuthProvider`.

Future orchestration can choose a provider by `Name`, or multiple providers can be registered and resolved as `IEnumerable<IAuthProvider>`.

## Architectural decisions
- Exposed provider-neutral request/response records (`AuthSignInRequest`, `AuthSignInResult`, `AuthSignOutResult`) to avoid leaking Identity types to higher layers.
- Provider `Name` values are stable identifiers intended for routing and configuration.
