# Copilot Instructions — Margie's Travel Agent

## Build & Run

```bash
# Build
dotnet build

# Run (dev, port 5296)
dotnet run --project MargiesTravelAgent --launch-profile http
```

## Tests

Tests are Playwright (NUnit) end-to-end tests that require the app running on `http://localhost:5296`.

```bash
# Install Playwright browsers (one-time)
pwsh MargiesTravelAgent.Tests/bin/Debug/net10.0/playwright.ps1 install

# Run all tests
dotnet test MargiesTravelAgent.Tests

# Run a single test
dotnet test MargiesTravelAgent.Tests --filter "ChatPage_LoadsSuccessfully"
```

The `BaseUrl` defaults to `http://localhost:5296` and can be overridden via `playwright.runsettings`.

## Architecture

ASP.NET Core 10 MVC app that wraps an Azure AI Foundry agent as a chat UI.

- **Default route** is `{controller=Chat}/{action=Index}/{id?}` — the chat page is the landing page, not Home.
- **`AzureAIChatService`** — singleton service that uses `Azure.AI.Projects` `AIProjectClient` + `ProjectResponsesClient` to send user messages (text + optional image) to the Foundry agent and extract response text. Conversation continuity uses `previousResponseId` chaining.
- **`ChatController.Send`** — POST endpoint accepting `ChatRequest` (message, optional image upload, previousResponseId) via form data with anti-forgery validation; returns `ChatResponse` as JSON.
- **Auth** — uses `DefaultAzureCredential` (no API keys in config). Azure AI connection settings live in `appsettings.json` under the `AzureAI` section, bound to `AzureAISettings` via the Options pattern.

## Conventions

- File-scoped namespaces (`namespace X;`) throughout.
- Nullable reference types enabled globally.
- Azure AI SDK warning `OPENAI001` is suppressed with `#pragma warning disable` at the top of `AzureAIChatService.cs`.
- Test classes inherit from `PlaywrightTestBase` (which extends `PageTest`) and use the `[Category("Visual")]` attribute.
