# Changelog

All notable changes to `AtolOnlineNet` will be documented in this file.

The project uses Semantic Versioning. Versions below `1.0.0` are preview releases and may include public API changes while the SDK contracts are validated against real integrations.

## Unreleased

No changes yet.

## 0.1.0-preview.1

Initial preview.

### Added

- `AtolOnlineClient` / `IAtolOnlineClient` for the ATOL Online cloud fiscalization API (`possystem` v4, ФФД 1.05):
  - `GetTokenAsync` — authorization with automatic token caching (24h) and transparent re-auth on `ExpiredToken`.
  - `RegisterAsync` / `RegisterCorrectionAsync` — register documents (FFD-version-neutral, request passed as `object`).
  - `GetReportAsync` — poll fiscalization results by UUID.
  - Typed FFD 1.05 helpers (`SellAsync`, `SellRefundAsync`, `BuyAsync`, `BuyRefundAsync`, `SellCorrectionAsync`, `BuyCorrectionAsync`) as extension methods in the `AtolOnlineNet.V1_05` namespace.
- Strongly-typed FFD 1.05 request models (`AtolOnlineNet.V1_05`) for the full receipt and correction trees (client, company, agent/supplier info, items, payments, VATs, cashless payments, additional props).
- Strongly-typed response models (`TokenResponse`, `RegisterDocumentResponse`, `DocumentReportResponse`, `ReportPayload`, `AtolError`).
- Enumerations for `sno`, `payment_method`, `payment_object`, VAT rates, agent types, payment types, correction types, document status and error types, with forward-compatible `Unknown` fallbacks.
- Exception hierarchy: `AtolOnlineApiException`, `AtolOnlineTransportException`, `AtolOnlineProtocolException`.
- Default `User-Agent` header (`AtolOnlineNet/<version>`) applied per-request unless the caller's `HttpClient` already configures one.
- Multi-targeting: `netstandard2.0`, `net8.0`, `net10.0`.
- FFD-version-neutral client design: the transport (token / register / report) is shared; FFD 1.05 models live in `AtolOnlineNet.V1_05`, leaving room for `AtolOnlineNet.V1_2` (ФФД 1.2) as a sibling.
