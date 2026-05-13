using GameProfile.Api.Resources.PlayerResource.Response;
using GameProfile.Api.Services;
using GameProfile.Core.Configuration;
using GameProfile.DataSetup;
using GameProfile.DB;
using GameProfile.DB.Models;
using GameProfile.Tests.Fixtures;
using GameProfile.Utils.Logging;
using RestSharp;
using Xunit;

namespace GameProfile.Tests;

[Collection(nameof(ApiTestCollection))]
public abstract class BaseTest : IDisposable
{
    protected readonly ILogger Logger;
    protected readonly AutomationTaskService AutomationTaskService;
    protected readonly InMemoryDatabase Db = new();

    protected Action RollBackAction { get; set; }

    protected BaseTest(LoginFixture loginFixture)
    {
        var testContext = TestContext.Current;
        Logger = new ConsoleLogger(testContext);
        Logger.Log($"Test '{testContext.TestCase?.TestCaseDisplayName}' execution started.");

        var settings = TestSettingsLoader.Settings;
        Logger.Log($"Environment: {settings.Environment}, API base URL: {settings.ApiBaseUrl}");

        AutomationTaskService = new AutomationTaskService(Logger) { Token = loginFixture.Token };
    }

    protected (RestResponse<PlayerResponse> Response, long Id) CreatePlayer(Player player)
    {
        var response = AutomationTaskService.CreatePlayer(
            player.Name, player.Age, player.Gender, player.Country);

        var id = response.Data?.Id
            ?? throw new InvalidOperationException(
                $"CreatePlayer returned no id; status={(int)response.StatusCode} {response.StatusCode}, body: {response.Content}");

        Db.Players.Insert(new PlayerRecord
        {
            Id = id,
            Name = player.Name,
            Age = player.Age,
            Gender = player.Gender,
            Country = player.Country
        });
        return (response, id);
    }

    protected Action DeleteAll(IEnumerable<long> ids)
    {
        var snapshot = ids?.ToList() ?? new List<long>();
        return () =>
        {
            foreach (var id in snapshot)
            {
                try { AutomationTaskService.DeletePlayer(id); }
                catch (Exception ex) { Logger.Log($"[ROLLBACK-ERROR] Cleanup failed for id {id}: {ex}"); }
            }
        };
    }

    public void Dispose()
    {
        try
        {
            Logger?.Log("==========================================");
            Logger?.Log($"Starting rollback for '{TestContext.Current.TestCase?.TestCaseDisplayName}'.");
            RollBackAction?.Invoke();
            Logger?.Log("Rollback completed.");
            Logger?.Log("==========================================");
        }
        catch (Exception ex)
        {
            Logger?.Log($"[ROLLBACK-ERROR] Error while rolling back: {ex}");
        }
    }
}
