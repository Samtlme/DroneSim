using DroneSim.Api.DTOs;
using DroneSim.Api.SignalR;
using DroneSim.Application.UseCases.Replay;
using DroneSim.Core.Entities;
using DroneSim.Core.Services;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace DroneSim.Api.Services;    
public class SwarmNotifier
{
    private int _frameOrder = 0;
    private string? _currentReplayId;
    private bool _isRecording = false;
    private readonly double _targetReplayFps = 2.0; //Limited to 2Hz, enough and shouldn't overwhelm the DB
    private readonly SwarmService _swarmService;
    private readonly ReplayManager _replayManager;
    private readonly IHubContext<DronesHub> _hubContext;
    private DateTime _lastFrameTime = DateTime.MinValue;
    public SwarmNotifier(SwarmService swarm, IHubContext<DronesHub> hubContext, ReplayManager replayManager)
    {
        _hubContext = hubContext;
        _replayManager = replayManager;
        _swarmService = swarm;

        _swarmService.OnDronesUpdated += async (drones) =>
        {
            var dtoList = new List<DroneDto>();
            lock (drones)
            {
                dtoList = drones.Select(x => new DroneDto()
                {
                    Id = x.Id,
                    X = x.Position.X,
                    Y = x.Position.Y,
                    Z = x.Position.Z
                }).ToList();
            }
            await _hubContext.Clients.All.SendAsync("UpdateDrones", dtoList);

            var now = DateTime.UtcNow;
            if (_isRecording && _currentReplayId != null && ((now - _lastFrameTime).TotalSeconds >= 1.0 / _targetReplayFps))
            {
                _lastFrameTime = now;
                var frame = new
                {
                    Timestamp = DateTime.UtcNow,
                    Frame = ++_frameOrder,
                    Drones = dtoList
                };
                await _replayManager.SaveFrameAsync(_currentReplayId, frame);
            }
        };
    }

    public async Task PlayReplayAsync(string replayId, double replayFps = 2.0, int chunkSize = 20, int currentIndex = 0)
    {
        var mainBuffer = new Queue<List<DroneDto>>();
        var auxBuffer = new Queue<List<DroneDto>>();
        bool isFetchingFrames = false;

        _swarmService.PauseSimulation();
        _swarmService.ClearDroneList();

        var initialChunk = await _replayManager.GetReplayChunkAsync<JsonElement>(replayId, currentIndex, chunkSize);
        foreach (var frame in initialChunk)
            mainBuffer.Enqueue(frame.GetProperty("Drones").Deserialize<List<DroneDto>>()!);

        currentIndex += chunkSize;

        while (mainBuffer.Count > 0)
        {
            var drones = mainBuffer.Dequeue();
            await _hubContext.Clients.All.SendAsync("UpdateDrones", drones);

            if (!isFetchingFrames && mainBuffer.Count <= chunkSize / 2)
            {
                isFetchingFrames = true;
                _ = Task.Run(async () =>
                {
                    var nextChunk = await _replayManager.GetReplayChunkAsync<JsonElement>(
                        replayId, currentIndex, chunkSize);
                    lock (auxBuffer)
                    {
                        foreach (var frame in nextChunk)
                            auxBuffer.Enqueue(frame.GetProperty("Drones").Deserialize<List<DroneDto>>()!);
                    }
                    currentIndex += chunkSize;
                    isFetchingFrames = false;
                });
            }

            if (mainBuffer.Count == 0)
            {
                lock (auxBuffer)
                {
                    while (auxBuffer.Count > 0)
                        mainBuffer.Enqueue(auxBuffer.Dequeue());
                }
            }

            await Task.Delay((int)(1000 / replayFps));
        }
    }

    public async Task<IReadOnlyList<ReplayInfo>> GetReplayList()
    {
        return await _replayManager.ListReplaysAsync();
    }

    public async Task DeleteReplayAsync(string sessionId)
    {
        await _replayManager.DeleteReplayAsync(sessionId);
    }

    public async Task StartRecording()
    {
        if (_isRecording)
            throw new InvalidOperationException("Error: Already recording.");

        _frameOrder = 0;
        _currentReplayId = Guid.NewGuid().ToString();
        await _replayManager.RegisterReplayAsync(_currentReplayId);
        _isRecording = true;
    }

    public void StopRecording()
    {
        _isRecording = false;
        _currentReplayId = null;
    }

}
