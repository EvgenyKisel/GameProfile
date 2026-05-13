using System.Net;
using GameProfile.DataSetup;
using GameProfile.Tests.Constants;
using GameProfile.Tests.Fixtures;
using GameProfile.Tests.Utils;
using Xunit;

namespace GameProfile.Tests.Tests;

[Trait(TraitName.Category, TestCategory.Players)]
public class GetPlayerTests(LoginFixture loginFixture) : BaseTest(loginFixture)
{
    [Fact]
    public void Player_GetOne_ReturnsCreatedPlayer_200()
    {
        var player = PlayerDataCreator.CreateRandomPlayer();
        var (_, createdId) = CreatePlayer(player);

        RollBackAction = DeleteAll(new[] { createdId });

        var response = AutomationTaskService.GetPlayer(createdId);
        var dbRecord = Db.Players.GetById(createdId);

        Assertions.Validate()
            .Equal(HttpStatusCode.OK, response.StatusCode, "GetOne status code should be 200.")
            .NotNull(response.Data, "GetOne response body should not be null.")
            .NotNull(dbRecord, "Player should be present in the simulated DB.")
            .Equal(dbRecord.Id, response.Data?.Id ?? -1, "GetOne id should match DB.")
            .Equal(dbRecord.Name, response.Data?.Name, "GetOne name should match DB.")
            .Equal(dbRecord.Age, response.Data?.Age, "GetOne age should match DB.")
            .Equal(dbRecord.Gender, response.Data?.Gender, "GetOne gender should match DB.")
            .Equal(dbRecord.Country, response.Data?.Country, "GetOne country should match DB.");
    }
}
