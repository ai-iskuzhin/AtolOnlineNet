# AGENTS.md — working agreement for AtolOnlineNet

This is a dependency-light .NET SDK for the **ATOL Online** (АТОЛ Онлайн) cloud fiscalization API.
It wraps the HTTP protocol; it contains no business rules about when or what to fiscalize.

## Scope

- **In scope (now):** `possystem` **v4** (ФФД 1.05) — authorization, document registration
  (`sell`, `sell_refund`, `buy`, `buy_refund`, `sell_correction`, `buy_correction`) and result polling.
- **Anticipated (later):** `possystem` **v5** (ФФД 1.2). The transport envelope (token / register /
  report) is shared across versions; only the receipt request models differ. Add v5 as a sibling
  request model — do **not** fork the client. The version is selected via
  `AtolOnlineClientOptions.ApiVersion`.
- **Out of scope:** the separate ATOL Online "информация о ККТ и ФН" monitoring API
  (`/api/kkt/v1/...`) — that is a different service (see `docs/`).

## Package boundary rules

- Keep the core package dependency-light: `HttpClient` + `System.Text.Json` + BCL only.
  No ASP.NET, no DI framework, no EF/ORM, no logging framework in the core library.
- DI is documented in the README via `IHttpClientFactory` typed-client registration; do not add
  `Microsoft.Extensions.*` dependencies to the core package.
- Public API is documented with XML comments; `CS1591` is an error (see `Directory.Build.targets`).

## Protocol rules

- All amounts are `decimal` (rubles). All timestamps use ATOL's `dd.MM.yyyy HH:mm:ss` format
  (`base_date` uses `dd.MM.yyyy`) via the converters in `Serialization/`.
- Enums serialize to their exact wire strings via `[EnumMember]` + `EnumMemberJsonConverter<T>`,
  with an `Unknown` member for forward compatibility. `payments[].type` is a numeric enum (0–9).
- JSON is `snake_case`, omits nulls, and does not escape Cyrillic (see `AtolJson`).
- The client caches the auth token for 24h and transparently re-authenticates once on `ExpiredToken`
  (service error code 11).
- Business outcomes (`wait` / `done` / `fail`, error envelopes) are returned on the response objects —
  they are not thrown. Only authentication failures, transport failures, and unparseable responses throw.

## Testing expectations

- **Unit tests** (`tests/AtolOnlineNet.Tests`) use a `RecordingHandler` over `HttpClient`; verify
  endpoint/URL, `Token` header, JSON field casing/values, token caching, and the expired-token retry.
- **Integration tests** (`tests/AtolOnlineNet.Tests.Integration`) run against the ATOL test environment
  only when credentials are configured (`.env` / env vars; see `.env.example`); otherwise they
  short-circuit so the suite stays green without secrets.

## Release discipline

- Semantic Versioning; preview versions `0.1.0-preview.N`. Tag format: `v0.1.0-preview.1`.
- Version is set in `Directory.Build.props` and overridden in CI via `-p:Version=`.
- NuGet publishing uses OIDC Trusted Publishing (`NuGet/login@v1`), no stored API key.
