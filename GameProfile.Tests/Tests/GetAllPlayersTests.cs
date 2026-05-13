using System.Net;
using GameProfile.DataSetup;
using GameProfile.Tests.Constants;
using GameProfile.Tests.Utils;
using Xunit;

namespace GameProfile.Tests.Tests;

[Trait(TraitName.Category, TestCategory.Players)]
public class GetAllPlayersTests : BaseTest
{
    [Fact]
    public void Player_GetAll_SortedByName_200()
    {
        const int seedCount = 5;
        var createdIds = new List<long>();
        RollBackAction = DeleteAll(createdIds);

        foreach (var player in PlayerDataCreator.CreateRandomPlayers(seedCount))
        {
            var (_, id) = CreatePlayer(player);
            createdIds.Add(id);
        }

        var response = AutomationTaskService.GetAllPlayers();

        Assertions.Validate()
            .Equal(HttpStatusCode.OK, response.StatusCode, "GetAll status code should be 200.")
            .NotNull(response.Data, "GetAll response body should not be null.")
            .NotEmpty(response.Data, "GetAll should return at least the players we created.");

        foreach (var dbPlayer in Db.Players.GetAll())
        {
            var apiPlayer = response.Data.FirstOrDefault(p => p.Id == dbPlayer.Id);
            Assertions.Validate()
                .NotNull(apiPlayer, $"DB player id={dbPlayer.Id} should appear in GetAll response.")
                .Equal(dbPlayer.Name, apiPlayer.Name, $"GetAll name mismatch for id={dbPlayer.Id}.")
                .Equal(dbPlayer.Age, apiPlayer.Age, $"GetAll age mismatch for id={dbPlayer.Id}.")
                .Equal(dbPlayer.Gender, apiPlayer.Gender, $"GetAll gender mismatch for id={dbPlayer.Id}.")
                .Equal(dbPlayer.Country, apiPlayer.Country, $"GetAll country mismatch for id={dbPlayer.Id}.");
        }

        var sorted = response.Data
            .OrderBy(p => p.Name, StringComparer.Ordinal)
            .ToList();

        Assertions.Validate()
            .SortedAscendingByName(sorted, p => p.Name, "Sorted list is not ascending by name.");

        Logger.Log($"Sorted players (count={sorted.Count}):");
        foreach (var p in sorted)
        {
            Logger.Log($"  id={p.Id}, name='{p.Name}', age={p.Age}");
        }
    }
}
