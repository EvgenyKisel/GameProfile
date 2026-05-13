using GameProfile.DB.Tables;

namespace GameProfile.DB;

public class InMemoryDatabase
{
    public PlayersTable Players { get; } = new();
}
