using GameProfile.Api.Resources.PlayerResource.Response;
using GameProfile.Api.Services;
using GameProfile.Core.Configuration;
using GameProfile.DataSetup;
using GameProfile.DB;
using GameProfile.DB.Models;
using GameProfile.Utils.Logging;
using RestSharp;
using Xunit;

namespace GameProfile.Tests;

public abstract class BaseTest : IDisposable
{
    protected readonly ILogger Logger;
    protected readonly TesterService TesterService;
    protected readonly AutomationTaskService AutomationTaskService;
    protected readonly InMemoryDatabase Db = new();

    protected Action RollBackAction { get; set; }

    protected BaseTest()
    {
        var settings = TestSettingsLoader.Settings;
        var testContext = TestContext.Current;
        Logger = new ConsoleLogger(testContext);
        Logger.Log($"Test '{testContext.TestCase?.TestCaseDisplayName}' execution started.");
        Logger.Log($"Environment: {settings.Environment}, API base URL: {settings.ApiBaseUrl}");

        TesterService = new TesterService(Logger);
        AutomationTaskService = new AutomationTaskService(Logger);

        var loginResponse = TesterService.Login(settings.Tester.Login, settings.Tester.Password);
        AutomationTaskService.Token = loginResponse.Data?.Token;

        if (string.IsNullOrEmpty(AutomationTaskService.Token))
        {
            Logger.Log("Warning: login succeeded but no token was returned.");
        }
    }

    protected (RestResponse<PlayerResponse> Response, long Id) CreatePlayer(Player player)
    {
        var response = AutomationTaskService.CreatePlayer(
            player.Name, player.Age, player.Gender, player.Country);

        var id = response.Data?.Id
            ?? throw new InvalidOperationException("Create returned no id; cannot continue.");

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

    protected Action DeleteAll(IEnumerable<long> ids) => () =>
    {
        foreach (var id in ids)
        {
            try { AutomationTaskService.DeletePlayer(id); }
            catch (Exception ex) { Logger.Log($"Cleanup failed for id {id}: {ex.Message}"); }
        }
    };

    public void Dispose()
    {
        try
        {
            Logger.Log("==========================================");
            Logger.Log($"Starting rollback for '{TestContext.Current.TestCase?.TestCaseDisplayName}'.");
            RollBackAction?.Invoke();
            Logger.Log("Rollback completed.");
            Logger.Log("==========================================");
        }
        catch (Exception ex)
        {
            Logger.Log($"Error while rolling back: {ex.Message}");
        }
    }
}
