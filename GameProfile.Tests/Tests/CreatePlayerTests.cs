using System.Net;
using GameProfile.DataSetup;
using GameProfile.Tests.Constants;
using GameProfile.Tests.Utils;
using Xunit;

namespace GameProfile.Tests.Tests;

[Trait(TraitName.Category, TestCategory.Players)]
public class CreatePlayerTests : BaseTest
{
    [Fact]
    public void Player_Create_12_201()
    {
        const int playerCount = 12;
        var createdIds = new List<long>();
        RollBackAction = DeleteAll(createdIds);

        foreach (var player in PlayerDataCreator.CreateRandomPlayers(playerCount))
        {
            var (createResponse, createdId) = CreatePlayer(player);
            createdIds.Add(createdId);

            Assertions.Validate()
                .Equal(HttpStatusCode.Created, createResponse.StatusCode, "Create should return 201.")
                .NotNull(createResponse.Data, "Create response body should not be null.")
                .Equal(player.Name, createResponse.Data?.Name, "Create response name should echo request.")
                .Equal(player.Age, createResponse.Data?.Age, "Create response age should echo request.")
                .Equal(player.Gender, createResponse.Data?.Gender, "Create response gender should echo request.")
                .Equal(player.Country, createResponse.Data?.Country, "Create response country should echo request.");

            var getResponse = AutomationTaskService.GetPlayer(createdId);
            var dbRecord = Db.Players.GetById(createdId);

            Assertions.Validate()
                .Equal(HttpStatusCode.OK, getResponse.StatusCode, "Post-create GET should return 200.")
                .NotNull(getResponse.Data, "Post-create GET body should not be null.")
                .Equal(dbRecord.Name, getResponse.Data?.Name, "Persisted name should match DB.")
                .Equal(dbRecord.Age, getResponse.Data?.Age, "Persisted age should match DB.")
                .Equal(dbRecord.Gender, getResponse.Data?.Gender, "Persisted gender should match DB.")
                .Equal(dbRecord.Country, getResponse.Data?.Country, "Persisted country should match DB.");
        }

        Assertions.Validate()
            .Count(createdIds, playerCount, $"Expected {playerCount} players to be created.")
            .Equal(playerCount, Db.Players.Count, "DB should mirror all created players.");
    }
}
