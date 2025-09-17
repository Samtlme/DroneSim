using DroneSim.Api.SignalR;
using DroneSim.Core.Services;
using Microsoft.AspNetCore.SignalR;

public class SwarmNotifier
{
    public SwarmNotifier(SwarmService swarm, IHubContext<DronesHub> hubContext)
    {
        swarm.OnDronesUpdated += async (drones) =>
        {
            await hubContext.Clients.All.SendAsync("UpdateDrones", drones);
        };
    }
}
