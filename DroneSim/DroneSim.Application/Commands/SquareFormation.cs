using DroneSim.Core.Configuration;
using DroneSim.Core.Interfaces;
using DroneSim.Core.Services;

namespace DroneSim.Application.Commands
{
    internal class SquareFormation : ICommand
    {
        private readonly SwarmService _swarm;
        private readonly PhysicsService _physics;
        public int Priority { get; } = 3;
        private bool _isCompleted = false;
        public string Name { get; } = "Square Formation";

        public bool IsCompleted => _isCompleted;
        public SquareFormation(SwarmService swarm, PhysicsService physics)
        {
            _swarm = swarm;
            _physics = physics;
        }

        public Task<bool> ExecuteAsync()
        {
            var total = _swarm.GetDroneList.Count;
            var quantityPerSide = (int)Math.Ceiling(Math.Sqrt(total));
            var initialpos = _physics.CalculateCenterOfMass(_swarm.GetDroneList);
            var initialX = initialpos.X;

            int counter = 1;
            foreach (var drone in _swarm.GetDroneList)
            {
                drone.PositionOffset = initialpos;
                initialpos.X += SimulationConfig.MinSeparationDistance;
                if (counter >= quantityPerSide)
                {
                    initialpos.X = initialX;
                    initialpos.Z += SimulationConfig.MinSeparationDistance;
                    counter = 0;
                }
                counter++;
            }
            return Task.FromResult(true);
        }
    }
}
