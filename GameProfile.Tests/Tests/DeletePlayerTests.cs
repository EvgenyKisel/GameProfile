using System.Net;
using GameProfile.DataSetup;
using GameProfile.Tests.Constants;
using GameProfile.Tests.Utils;
using Xunit;

namespace GameProfile.Tests.Tests;

[Trait(TraitName.Category, TestCategory.Players)]
public class DeletePlayerTests : BaseTest
{
    [Fact]
    public void Player_DeleteAllCreated_200()
    {
        const int seedCount = 12;
        var createdIds = new List<long>();

        foreach (var player in PlayerDataCreator.CreateRandomPlayers(seedCount))
        {
            var (_, id) = CreatePlayer(player);
            createdIds.Add(id);
        }

        Assertions.Validate()
            .Count(createdIds, seedCount, "Pre-condition: all seed players should have been created.");

        foreach (var id in createdIds)
        {
            var deleteResponse = AutomationTaskService.DeletePlayer(id);
            Db.Players.Delete(id);

            Assertions.Validate()
                .Equal(HttpStatusCode.OK, deleteResponse.StatusCode, $"Delete status for id {id} should be 200.")
                .Null(Db.Players.GetById(id), $"DB should no longer contain id {id}.");
        }

        var allAfterDelete = AutomationTaskService.GetAllPlayers();
        var leakedIds = allAfterDelete.Data
            .Where(p => p.Id is long apiId && createdIds.Contains(apiId))
            .Select(p => p.Id)
            .ToList();

        Assertions.Validate()
            .Count(leakedIds, 0, $"Deleted ids still in GetAll: [{string.Join(",", leakedIds)}].")
            .Equal(0, Db.Players.Count, "DB should be empty after all deletes.");
    }
}
