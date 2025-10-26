using DroneSim.Core.Entities;
using System.Text.Json;
namespace DroneSim.Core.Interfaces;

public interface IReplayRepository
{
    Task DeleteReplayAsync(string sessionId);
    Task<IReadOnlyList<JsonElement>> GetReplayChunkAsync<JsonElement>(string sessionId, int startIndex, int count);
    Task<IReadOnlyList<ReplayInfo>> ListReplaysAsync();
    Task RegisterReplayAsync(string sessionId);
    Task SaveFrameAsync(string sessionId, object frame);
}
