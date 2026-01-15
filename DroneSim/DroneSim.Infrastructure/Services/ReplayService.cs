using DroneSim.Core.Entities;
using DroneSim.Core.Interfaces;
using DroneSim.Infrastructure.Extensions;
using StackExchange.Redis;
using System.Text.Json;
namespace DroneSim.Infrastructure.Services;

public class ReplayService(IConnectionMultiplexer redis) : IReplayRepository
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task RegisterReplayAsync(string sessionId)
    {
        var replayMeta = new ReplayInfo()
        {
            Id = sessionId,
            StartedAt = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(replayMeta);
        await _db.ListRightPushAsync("replayList", json);
    }
    public async Task<IReadOnlyList<ReplayInfo>> ListReplaysAsync()
    {
        var items = await _db.ListRangeAsync("replayList");

        return items.Select(x => x.DeserializeJson<ReplayInfo>()).ToList();
    }
    public async Task DeleteReplayAsync(string sessionId)
    {
        await _db.KeyDeleteAsync($"replay:{sessionId}");
        var allReplays = await _db.ListRangeAsync("replayList");

        foreach (var entry in allReplays)
        {
            var replay = entry.DeserializeJson<ReplayInfo>();
            if (replay != null && replay.Id == sessionId)
            {
                await _db.ListRemoveAsync("replayList", entry);
                break;
            }
        }
    }
    public async Task SaveFrameAsync(string sessionId, object frame)
    {
        var json = JsonSerializer.Serialize(frame);
        await _db.ListRightPushAsync($"replay:{sessionId}", json);
        await _db.ListTrimAsync($"replay:{sessionId}", -2000, -1);  //Limit to last 2k frames, 1000s at 2Hz, to prevent accidental growth
    }

    // DISCLAIMER: Could switch to binary serialization (MemoryPack or Protobuf) for better performance 
    // and lower storage impact (by far), but JSON is fine for portfolio purposes and makes debugging easier

    public async Task<IReadOnlyList<T>> GetReplayChunkAsync<T>(string sessionId, int startIndex, int count)
    {
        var endIndex = startIndex + count - 1;
        var frames = await _db.ListRangeAsync($"replay:{sessionId}", startIndex, endIndex);
        return frames.Select(x => x.DeserializeJson<T>()).ToList();
    }

}
