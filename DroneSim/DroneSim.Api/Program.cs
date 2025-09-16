using DroneSim.Core.Services;

var builder = WebApplication.CreateBuilder(args);

//DI and services
builder.Services.AddSingleton<SwarmService>();
builder.Services.AddSignalR();


var app = builder.Build();

app.MapHub<DroneSim.Api.SignalR.DronesHub>("/droneshub");

app.MapPost("/Api/Simulation/Start", (SwarmService swarm) =>
{
    swarm.StartSimulation();
    return Results.Ok("Simulation started");
});

app.MapPost("/Api/Simulation/Pause", (SwarmService swarm) =>
{
    swarm.PauseSimulation();
    return Results.Ok("Simulation paused");
});


app.Run();
