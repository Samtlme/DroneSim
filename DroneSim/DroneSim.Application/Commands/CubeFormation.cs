using DroneSim.Core.Configuration;
using DroneSim.Core.Interfaces;
using DroneSim.Core.Services;

namespace DroneSim.Application.Commands
{
    internal class CubeFormation : ICommand
    {
        private readonly SwarmService _swarm;
        private readonly PhysicsService _physics;
        public int Priority { get; } = 3;
        private bool _isCompleted = false;
        public string Name { get; } = "Square Formation";

        public bool IsCompleted => _isCompleted;
        public CubeFormation(SwarmService swarm, PhysicsService physics)
        {
            _swarm = swarm;
            _physics = physics;
        }

        public Task<bool> ExecuteAsync()
        {
            var total = _swarm.GetDroneList.Count;
            var quantityPerSide = (int)Math.Ceiling(Math.Cbrt(total));
            var initialpos = _physics.CalculateCenterOfMass(_swarm.GetDroneList);

            //check if it fits
            if (initialpos.Y + ((quantityPerSide - 1) * SimulationConfig.MinSeparationDistance) > SimulationConfig.YMax) 
            {
                initialpos.Y = SimulationConfig.YMax - ((quantityPerSide - 1) * SimulationConfig.MinSeparationDistance);
            }

            var initialX = initialpos.X;
            var initialZ = initialpos.Z;

            int counterX = 1;
            int counterZ = 0;

            foreach (var drone in _swarm.GetDroneList)
            {
                drone.PositionOffset = initialpos;
                initialpos.X += SimulationConfig.MinSeparationDistance;
                if (counterX >= quantityPerSide)
                {
                    initialpos.X = initialX;
                    initialpos.Z += SimulationConfig.MinSeparationDistance;
                    counterX = 0;
                    counterZ++;
                }
                counterX++;

                if (counterZ >= quantityPerSide) 
                {
                    counterX = 1;
                    counterZ = 0;
                    initialpos.Z = initialZ;
                    initialpos.Y += SimulationConfig.MinSeparationDistance;
                }
            }
            return Task.FromResult(true);
        }
    }
}
