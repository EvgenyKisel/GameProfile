using GameProfile.Utils.Utilities;

namespace GameProfile.DataSetup;

public static class PlayerDataCreator
{
    public static Player CreateRandomPlayer() => new()
    {
        Name = RandomGenerator.GetFullName(),
        Age = RandomGenerator.GetInt(18, 60),
        Gender = RandomGenerator.GetGender(),
        Country = RandomGenerator.GetCountry()
    };

    public static List<Player> CreateRandomPlayers(int count) =>
        Enumerable.Range(0, count).Select(_ => CreateRandomPlayer()).ToList();
}
