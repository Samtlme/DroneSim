using DroneSim.Core.Entities;
using DroneSim.Core.Services;
using System.Numerics;

namespace DroneSim.Application.UseCases.Simulation
{
    public class SimulationManager
    {
        private readonly SwarmService _swarmService;

        public SimulationManager(SwarmService swarmService)
        {
            _swarmService = swarmService;
        }

        public void StartSimulation(int droneCount = 1500, int refreshRate = 60) //60Hz by default, 1500 drones
        {
            _swarmService.StartSimulation(droneCount, refreshRate);
        }

        public void PauseSimulation() => _swarmService.PauseSimulation();
        
    }
}
