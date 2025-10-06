using DroneSim.Application.Commands;
using DroneSim.Core.Entities;
using DroneSim.Core.Services;
using System.Numerics;

namespace DroneSim.Application.UseCases.Swarm
{
    public class SwarmCommandManager
    {
        private readonly PhysicsService _physicsService;
        private readonly CommandService _commandService;
        private readonly SwarmService _swarmService;

        public SwarmCommandManager(PhysicsService physicsService, CommandService commandService, SwarmService swarmService)
        {
            _physicsService = physicsService;
            _commandService = commandService;
            _swarmService = swarmService;
        }
        public void MoveToTarget(Coordinates target) 
        {
            _commandService.EnqueueCommand(
                new MoveToTarget(
                    _swarmService,
                    _physicsService, 
                    new Vector3() { X = target.X, Y = target.Y, Z = target.Z }
                )
            );
        } 
    

    }
}
