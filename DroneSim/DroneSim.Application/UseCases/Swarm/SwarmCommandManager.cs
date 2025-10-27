using DroneSim.Application.Commands;
using DroneSim.Core.Configuration;
using DroneSim.Core.Entities;
using DroneSim.Core.Services;
using System.Numerics;

namespace DroneSim.Application.UseCases.Swarm;

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
    public void GetInCustomFormation(List<List<Coordinates>> pointList)
    {
        var pointListVectors = pointList
            .Select(x => x.Select(p => new Vector2(p.X, p.Y)).ToList())
            .ToList();

        _commandService.EnqueueCommand(
            new CustomFormation(_swarmService, pointListVectors)
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

    public void MoveDronesUp()
    {
        var drones = _swarmService.GetDroneList;
        var movement = new Vector3(0, 4, 0);
        foreach (var drone in drones) 
        {
            drone.Position += movement;
            if (drone.PositionOffset != Vector3.Zero) 
            {
                drone.PositionOffset += movement;
            }
        }
    }

    public void MoveDronesDown()
    {
        var drones = _swarmService.GetDroneList;
        var movement = new Vector3(0, 4, 0);
        foreach (var drone in drones)
        {
            drone.Position -= new Vector3(0, 4, 0);
            if (drone.PositionOffset != Vector3.Zero)
            {
                drone.PositionOffset -= movement;
            }
        }
    }
}
