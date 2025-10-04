using DroneSim.Api.DTOs;
using DroneSim.Api.SignalR;
using DroneSim.Core.Services;
using Microsoft.AspNetCore.SignalR;

public class SwarmNotifier
{
    public SwarmNotifier(SwarmService swarm, IHubContext<DronesHub> hubContext)
    {
        swarm.OnDronesUpdated += async (drones) =>
        {
            var dtoList = drones.Select(x => new DroneDto()
            {
                Id = x.Id,
                X = x.Position.X,
                Y = x.Position.Y,
                Z = x.Position.Z
            });
            await hubContext.Clients.All.SendAsync("UpdateDrones", dtoList);
        };
    }
}
