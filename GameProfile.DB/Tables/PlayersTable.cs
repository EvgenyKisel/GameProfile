using System.Collections.Concurrent;
using GameProfile.DB.Models;

namespace GameProfile.DB.Tables;

public class PlayersTable
{
    private readonly ConcurrentDictionary<long, PlayerRecord> _rows = new();

    public PlayerRecord GetById(long id) =>
        _rows.TryGetValue(id, out var record) ? record : null;

    public IReadOnlyList<PlayerRecord> GetAll() =>
        _rows.Values.ToList();

    public void Insert(PlayerRecord record) =>
        _rows[record.Id] = record;

    public bool Delete(long id) =>
        _rows.TryRemove(id, out _);

    public int Count => _rows.Count;
}
