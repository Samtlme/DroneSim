using DroneSim.Core.Entities;
using DroneSim.Core.Services;
using System.Numerics;

namespace DroneSim.Application.UseCases
{
    public class SimulationManager
    {
        private readonly SwarmService _swarmService;

        public SimulationManager(SwarmService swarmService)
        {
            _swarmService = swarmService;
        }

        public void StartSimulation(int droneCount = 1500, int refreshRate = 1) //1Hz by default, 1500 drones
        {
            _swarmService.StartSimulation(droneCount, refreshRate);
        }

        public void PauseSimulation() => _swarmService.PauseSimulation();

        public void MoveToTarget(Coordinates target) => _swarmService.MoveToTarget(new Vector3(target.X,target.Y, target.Z));
    }
}
