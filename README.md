# GameProfile

This is my solution to the GameProfile QA automation task: drive five API scenarios end-to-end and keep the framework readable enough that adding another endpoint isn't painful.

It's plain .NET 10 + xUnit v3 with RestSharp under the hood. Nothing exotic.

## What's in here

The code is split across six small projects so each one has a single job:

- **`GameProfile.Core`** — the HTTP plumbing. An attribute-driven `HttpRequest` (mark fields with `[Body]`, `[Header]`, `[UrlParameter]`) and a thin `HttpClient` wrapper over RestSharp. Configuration loading also lives here so any project can read `TestSettings`.
- **`GameProfile.Api`** — endpoints, DTOs, and the service layer (`TesterService`, `AutomationTaskService`). Tests talk to services, not RestSharp directly.
- **`GameProfile.DataSetup`** — test data builders. Currently just `Player` and `PlayerDataCreator`, but isolated so it can grow.
- **`GameProfile.DB`** — a tiny in-memory "database" I use as the source of truth when validating GET responses. Lets tests cross-check API state without depending on a real DB.
- **`GameProfile.Utils`** — logger, random helpers, attribute reflection helpers.
- **`GameProfile.Tests`** — the xUnit v3 project. `BaseTest` handles login + cleanup, the `Assertions.Validate()` chain keeps test bodies short.

## Running it

```bash
cp GameProfile.Tests/.env.template GameProfile.Tests/.env
# fill in ENVIRONMENT, TESTER_LOGIN, TESTER_PASSWORD

dotnet build
dotnet run --project GameProfile.Tests
```

The runner is xUnit v3's in-process console runner, **not** `dotnet test`. v3 dropped support for the classic test SDK pipeline, so the project is its own executable. Filter tests with the usual flags:

```bash
dotnet run --project GameProfile.Tests -- -list tests
dotnet run --project GameProfile.Tests -- -method "*Tester_Login_200*"
dotnet run --project GameProfile.Tests -- -trait "Category=Players"
dotnet run --project GameProfile.Tests -- -class "*GetAllPlayersTests*"
```

## Configuration

I wanted secrets out of source control and per-environment URLs in. Three sources, layered from lowest to highest priority:

1. `appsettings.{environment}.json` — non-secret per-env values (`ApiBaseUrl`, etc.). Shipped: `appsettings.dev.json` and `appsettings.staging.json`. Drop a new one in to add an environment.
2. `.env` — the `ENVIRONMENT` selector plus `TESTER_LOGIN` / `TESTER_PASSWORD`. Gitignored.
3. `GAMEPROFILE_*` environment variables — what CI sets. Examples: `GAMEPROFILE_Environment=Dev`, `GAMEPROFILE_ApiBaseUrl=...`, `GAMEPROFILE_Tester__Password=...`.

Environment selection: `GAMEPROFILE_Environment` wins, then `ENVIRONMENT` from `.env`, then a `"Dev"` fallback hard-coded in `TestSettingsLoader`.

## The five scenarios

Straight mapping from the task spec to a test:

| # | Scenario | Test | Endpoint |
|---|---|---|---|
| 1 | Authenticate, get tester token | `TesterLoginTests.Tester_Login_200` | `POST /api/tester/login` |
| 2 | Register 12 players | `CreatePlayerTests.Player_Create_12_201` | `POST /api/automationTask/create` |
| 3 | Read a single created player | `GetPlayerTests.Player_GetOne_ReturnsCreatedPlayer_200` | `GET /api/automationTask/getOne?id=<id>` |
| 4 | List players, verify alphabetical sort | `GetAllPlayersTests.Player_GetAll_SortedByName_200` | `GET /api/automationTask/getAll` |
| 5 | Delete every created player | `DeletePlayerTests.Player_DeleteAllCreated_200` | `DELETE /api/automationTask/deleteOne/{id}` |

`BaseTest` logs in once per test, stashes the token on `AutomationTaskService`, and exposes two helpers: `CreatePlayer(player)` (creates via API and mirrors into the in-memory DB, returns the response + id) and `DeleteAll(ids)` (returns an `Action` you assign to `RollBackAction` for teardown).

## A note on the API shape

The task spec didn't include an OpenAPI doc, so the DTO field names and a few response codes are best guesses based on the endpoint names. If the real API uses different names or shapes, the places to adjust:

- `LoginRequest` / `LoginResponse` — expects `{ "login", "password" }` in and `token` back. Rename the `[JsonProperty]` if needed.
- `BaseRequest.Authorization` — sets `Authorization: <token>` with no `Bearer` prefix. Change the attribute if the API expects `Bearer ` or a custom header.
- `PlayerResponse` — assumes `id`, `name`, `age`, `gender`, `country`.
- `GetPlayerRequest` — sends `?id=<id>` as a query parameter. Swap to a path/body param if the real API differs.
- `AutomationTaskService.DeletePlayer` — asserts `200 OK`. If delete returns `204 No Content`, change the expected status in `ValidateResponse(...)`.

## Adding an endpoint

1. URL goes in `GameProfile.Api/ApiUrl.cs`.
2. Request/response DTOs under `GameProfile.Api/Resources/<Resource>/`.
3. Add a method to the matching `<Resource>ResourceApi`, expose a higher-level call from a service.
4. Test class inherits from `BaseTest` and uses the fluent assertions:

```csharp
[Trait(TraitName.Category, TestCategory.Players)]
public class MyTests : BaseTest
{
    [Fact]
    public void MyScenario_200()
    {
        var player = PlayerDataCreator.CreateRandomPlayer();
        var (createResponse, createdId) = CreatePlayer(player);
        RollBackAction = DeleteAll(new[] { createdId });

        Assertions.Validate()
            .Equal(HttpStatusCode.Created, createResponse.StatusCode)
            .NotNull(createResponse.Data);
    }
}
```
