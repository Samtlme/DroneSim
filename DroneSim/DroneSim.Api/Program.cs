using DroneSim.Api.Services;
using DroneSim.Api.SignalR;
using DroneSim.Application.UseCases.Replay;
using DroneSim.Application.UseCases.Simulation;
using DroneSim.Application.UseCases.Swarm;
using DroneSim.Core.Entities;
using DroneSim.Core.Interfaces;
using DroneSim.Core.Services;
using DroneSim.Infrastructure.Middleware;
using DroneSim.Infrastructure.Redis;
using DroneSim.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });
}

//DI and services

builder.Services.AddSingleton(new RedisConnectionFactory("localhost:6379"));
builder.Services.AddSingleton<IConnectionMultiplexer>(x =>
    x.GetRequiredService<RedisConnectionFactory>().GetConnection()
);

builder.Services.AddSignalR();
builder.Services.AddSingleton<PhysicsService>();
builder.Services.AddSingleton<CommandService>();
builder.Services.AddSingleton<SwarmService>();
builder.Services.AddSingleton<SwarmNotifier>();
builder.Services.AddSingleton<IReplayRepository, ReplayService>();
builder.Services.AddSingleton<ReplayManager>();
builder.Services.AddSingleton<SimulationManager>();
builder.Services.AddSingleton<SwarmCommandManager>();

var app = builder.Build();

var swarmNotifier = app.Services.GetRequiredService<SwarmNotifier>();

if (app.Environment.IsDevelopment())
{
    app.UseCors();
}

app.UseMiddleware<GlobalErrorHandler>();
app.UseHttpsRedirection();

app.MapHub<DronesHub>("/Api/Simulation/droneshub");

#region Simulation Control

app.MapPost("/Api/Simulation/Start", (SimulationManager swarmSM, [FromBody] int droneCount) =>
{
    swarmSM.StartSimulation(droneCount);
    return Results.Ok("Simulation started");
});

app.MapPost("/Api/Simulation/Pause", (SimulationManager swarmSM) =>
{
    swarmSM.PauseSimulation();
    return Results.Ok("Simulation paused");
});

app.MapPost("/Api/Simulation/setConfig", (SimulationManager swarmSM, SimulationConfigDto newConfig) =>
{
    try
    {
        swarmSM.SetConfiguration(newConfig);
        return Results.Ok("Settings modified");
    }
    catch (Exception ex)
    {
        return Results.Problem(detail:"Failed to apply configuration: " + ex.Message, statusCode: 400);
    }
});

#endregion

#region Commands and Formations

app.MapPost("/Api/Simulation/movetotarget", (SwarmCommandManager swarmCM, Coordinates target) =>
{
    swarmCM.MoveToTarget(target);
    return Results.Ok($"Movement requested to coordinates =>  x: {target.X} y: {target.Y} z: {target.Z}");
});

app.MapPost("/Api/Simulation/resetformation", (SwarmCommandManager swarmCM) =>
{
    swarmCM.ResetFormation();
    return Results.Ok($"Formation disbanded.");

});

app.MapPost("/Api/Simulation/customformation", (List<List<Coordinates>> pointList, SwarmCommandManager swarmCM) =>
{
    swarmCM.GetInCustomFormation(pointList);
    return Results.Ok($"Custom formation requested.");

});

app.MapPost("/Api/Simulation/Square", (SwarmCommandManager swarmCM) =>
{
    swarmCM.GetInSquareFormation();
    return Results.Ok($"Square formation requested");
});

app.MapPost("/Api/Simulation/Cube", (SwarmCommandManager swarmCM) =>
{
    swarmCM.GetInCubeFormation();
    return Results.Ok($"Cube formation requested");
});

app.MapPost("/Api/Simulation/MirrorToVertical", (SwarmCommandManager swarmCM) =>
{
    swarmCM.MirrorToVertical();
    return Results.Ok($"Cube formation requested");
});

app.MapPost("/Api/Simulation/dronesUp", (SwarmCommandManager swarmCM) =>
{
    swarmCM.MoveDronesUp();
    return Results.Ok($"Cube formation requested");

});

app.MapPost("/Api/Simulation/dronesDown", (SwarmCommandManager swarmCM) =>
{
    swarmCM.MoveDronesDown();
    return Results.Ok($"Cube formation requested");
});
#endregion


#region Replays

app.MapPost("/Api/Replay/start", (SwarmNotifier notifier) =>
{
    var id = notifier.StartRecording();
    return Results.Ok(new { ReplayId = id });
});

app.MapPost("/Api/Replay/stop", (SwarmNotifier notifier) =>
{
    notifier.StopRecording();
    return Results.Ok();
});

app.MapPost("/Api/Replay/getReplays", async (SwarmNotifier notifier) =>
{
    var replays = await notifier.GetReplayList();
    return Results.Ok(replays);
});

app.MapPost("/Api/Replay/playReplay", async (SwarmNotifier notifier, SimulationManager swarmSM, [FromBody] string replayId) =>
{
    swarmSM.StartSimulation(0);
    await notifier.PlayReplayAsync(replayId);
    return Results.Ok();
});

app.MapPost("/Api/Replay/deleteReplay", async (SwarmNotifier notifier, [FromBody] string replayId) =>
{
    await notifier.DeleteReplayAsync(replayId);
    return Results.Ok();
});

#endregion

app.Run();
