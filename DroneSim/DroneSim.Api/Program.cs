var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//Only endpoints related to simulation

app.MapPost("/Api/Simulation/Start", () => "Starting simulation...");
app.MapPost("/Api/Simulation/Pause", () => "Pausing simulation...");
app.MapGet("/Api/Simulation/Update", () => "Updating simulation...");

app.Run();
