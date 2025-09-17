using DroneSim.Api.SignalR;
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
builder.Services.AddSingleton<SwarmService>();
builder.Services.AddSingleton<SwarmNotifier>();


var app = builder.Build();

var swarmNotifier = app.Services.GetRequiredService<SwarmNotifier>();

if (app.Environment.IsDevelopment())
{
    app.UseCors();
}

app.MapHub<DronesHub>("/Api/Simulation/droneshub");

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
