using DroneSim.Core.Entities;
using DroneSim.Core.Interfaces;
using DroneSim.Core.Services;
using System.Numerics;

namespace DroneSim.Application.Commands
{
    public class MoveToTarget : ICommand
    {
        private readonly Vector3 _target;
        private readonly SwarmService _swarm;
        private readonly PhysicsService _physics;
        public int Priority { get; } = 5;
        private bool _isCompleted = false;
        public string Name { get; } = "Move to target";

        public bool IsCompleted => _isCompleted;
        public MoveToTarget(SwarmService swarm, PhysicsService physics, Vector3 target)
        {
            _target = target;
            _swarm = swarm;
            _physics = physics;
        }

        public Task<bool> ExecuteAsync()
        {
            var centerOfMass = _physics.CalculateCenterOfMass(_swarm.GetDroneList);
            var reached = _physics.MoveSwarmTowardsTarget(_swarm.GetDroneList, centerOfMass, _target);
            return Task.FromResult(reached);
        }
    }
}
