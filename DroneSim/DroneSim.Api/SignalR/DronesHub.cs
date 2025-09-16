using DroneSim.Core.Services;
using Microsoft.AspNetCore.SignalR;


namespace DroneSim.Api.SignalR
{
    public class DronesHub : Hub
    {
        public DronesHub(SwarmService swarmService)
        {
            swarmService.OnDronesUpdated += async (drones) =>
            {
                await this.Clients.All.SendAsync("UpdateDrones", drones);
            };
        }

    }
}
