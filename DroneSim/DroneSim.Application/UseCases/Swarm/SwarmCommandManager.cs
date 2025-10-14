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

        public void ResetFormation() 
        {
            foreach (var drone in _swarmService.GetDroneList) 
            {
                drone.PositionOffset = Vector3.Zero;
            }
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

        public void GetInSquareFormation()
        {
            _commandService.EnqueueCommand(
                new SquareFormation(
                    _swarmService,
                    _physicsService
                )
            );
        }

        public void GetInCubeFormation()
        {
            _commandService.EnqueueCommand(
                new CubeFormation(
                    _swarmService,
                    _physicsService
                )
            );
        }

    }
}
