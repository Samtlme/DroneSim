using DroneSim.Application.Commands;
using DroneSim.Core.Configuration;
using DroneSim.Core.Entities;
using DroneSim.Core.Services;
using System.Numerics;

namespace DroneSim.Application.UseCases.Swarm
{
    public class SwarmCommandManager(PhysicsService physicsService, CommandService commandService, SwarmService swarmService)
    {
        private readonly PhysicsService _physicsService = physicsService;
        private readonly CommandService _commandService = commandService;
        private readonly SwarmService _swarmService = swarmService;

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
        public void GetInCustomFormation(List<Coordinates> points)
        {
            _commandService.EnqueueCommand(
                new CustomFormation(
                    _swarmService,
                    points.Select(x => new Vector2(x.X, x.Y)).ToList()
                )
            );
        }

        public void MirrorToVertical()
        {
            var drones = _swarmService.GetDroneList;
            var centerOfMass = _physicsService.CalculateCenterOfMass(drones);

            List<Vector3> rotatedOffsets = new List<Vector3>();
            float lowestY = float.MaxValue;

            foreach (var drone in drones)
            {
                Vector3 relative = drone.PositionOffset - centerOfMass;
                relative = new Vector3(relative.X, -relative.Z, relative.Y);
                Vector3 rotated = centerOfMass + relative;
                rotatedOffsets.Add(rotated);

                if (rotated.Y < lowestY)
                    lowestY = rotated.Y;
            }

            float offsetY = SimulationConfig.YMin - lowestY;
            for (int i = 0; i < drones.Count; i++)
            {
                drones[i].PositionOffset = rotatedOffsets[i] + new Vector3(0, offsetY, 0);
            }
        }
    }
}
