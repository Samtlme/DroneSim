using DroneSim.Core.Interfaces;
using DroneSim.Core.Services;
using System.Numerics;

namespace DroneSim.Application.Commands;

public class MoveToTarget(SwarmService swarm, PhysicsService physics, Vector3 target) : ICommand
{
    private Vector3 _target = target;
    private readonly SwarmService _swarm = swarm;
    private readonly PhysicsService _physics = physics;

    public int Priority { get; } = 5;
    public string Name { get; } = "Move to target";
    public Task<bool> ExecuteAsync()
    {
        var centerOfMass = _physics.CalculateCenterOfMass(_swarm.GetDroneList);
        _target.Y = centerOfMass.Y; //TODO review or allow config - lock flight height
        var reached = _physics.MoveSwarmTowardsTarget(_swarm.GetDroneList, centerOfMass, _target);
        return Task.FromResult(reached);
    }
}
