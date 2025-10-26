using DroneSim.Core.Entities;
using DroneSim.Core.Interfaces;
using System.Text.Json;

namespace DroneSim.Application.UseCases.Replay;

public class ReplayManager(IReplayRepository replayService)
{
    private readonly IReplayRepository _replayService = replayService;


    public async Task DeleteReplayAsync(string sessionId)
    {
       await _replayService.DeleteReplayAsync(sessionId);
    }

    public async Task<IReadOnlyList<JsonElement>> GetReplayChunkAsync<JsonElement>(string sessionId, int startIndex, int count)
    {
        return await _replayService.GetReplayChunkAsync<JsonElement>(sessionId, startIndex, count);
    }

    public async Task<IReadOnlyList<ReplayInfo>> ListReplaysAsync()
    {
        return await _replayService.ListReplaysAsync();
    }

    public async Task RegisterReplayAsync(string currentReplayId)
    {
        await _replayService.RegisterReplayAsync(currentReplayId);
    }

    public async Task SaveFrameAsync(string currentReplayId, object frame)
    {
        await _replayService.SaveFrameAsync(currentReplayId, frame);
    }
}
