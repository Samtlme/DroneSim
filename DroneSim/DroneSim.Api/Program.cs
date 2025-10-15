using DroneSim.Api.SignalR;
using DroneSim.Application.UseCases.Simulation;
using DroneSim.Application.UseCases.Swarm;
using DroneSim.Core.Entities;
using DroneSim.Core.Services;

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
builder.Services.AddSignalR();
builder.Services.AddSingleton<PhysicsService>();
builder.Services.AddSingleton<CommandService>();
builder.Services.AddSingleton<SwarmService>();
builder.Services.AddSingleton<SwarmNotifier>();
builder.Services.AddSingleton<SimulationManager>();
builder.Services.AddSingleton<SwarmCommandManager>();


var app = builder.Build();

var swarmNotifier = app.Services.GetRequiredService<SwarmNotifier>();

if (app.Environment.IsDevelopment())
{
    app.UseCors();
}

app.MapHub<DronesHub>("/Api/Simulation/droneshub");

app.MapPost("/Api/Simulation/Start", (SimulationManager swarmSM) =>
{
    swarmSM.StartSimulation();
    return Results.Ok("Simulation started");
});

app.MapPost("/Api/Simulation/Pause", (SimulationManager swarmSM) =>
{
    swarmSM.PauseSimulation();
    return Results.Ok("Simulation paused");
});

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

app.MapPost("/Api/Simulation/customformation", (List<Coordinates> points, SwarmCommandManager swarmCM) =>
{
    swarmCM.GetInCustomFormation(points);
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

app.Run();
