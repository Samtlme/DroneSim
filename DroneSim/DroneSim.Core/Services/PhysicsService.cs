using DroneSim.Core.Configuration;
using DroneSim.Core.Entities;
using System.Numerics;

namespace DroneSim.Core.Services
{
    public class PhysicsService
    {
        internal void UpdatePositions(IEnumerable<Drone> drones)
        {
            ApplyBoidsRules(drones);
            SimulateWindEffect(drones); //The chaos we need to tune the boids parameters
        }

        /// <summary>
        /// Apply basic boids rules to a list of drones.
        /// For the sake of simplicity, we only implement separation and cohesion.
        /// No alignment is implemented, neither angular velocity limits nor obstacle avoidance.
        /// </summary>
        /// <param name="drones">Drone list</param>
        internal void ApplyBoidsRules(IEnumerable<Drone> drones)
        {
            foreach (var drone in drones)
            {
                var movement = Vector3.Zero;

                var neighbors = drones
                    .Where(d => d != drone &&
                                Vector3.DistanceSquared(d.Position, drone.Position) <
                                SimulationConfig.MinSeparationDistanceSquared * SimulationConfig.PerceptionFactor)
                    .ToList();

                //Separation
                foreach (var n in neighbors)
                {
                    var offset = drone.Position - n.Position;
                    var distanceSquared = Vector3.DistanceSquared(drone.Position, n.Position);

                    if (distanceSquared < SimulationConfig.MinSeparationDistanceSquared && distanceSquared > 0)
                    {
                        movement += offset / distanceSquared * SimulationConfig.SeparationSpeedFactor;
                    }
                }

                //Cohesion
                if (neighbors.Any())
                {
                    var neighborMassCenter = Vector3.Zero;

                    foreach (var n in neighbors)
                        neighborMassCenter += n.Position;

                    neighborMassCenter /= neighbors.Count;
                    movement += (neighborMassCenter - drone.Position) * SimulationConfig.CohesionSpeedFactor;

                }

                //Speed limit
                var speed = movement.Length();
                if (SimulationConfig.MaxDroneSpeedLimit > 0 && speed > SimulationConfig.MaxDroneSpeedLimit)
                {
                    movement = Vector3.Normalize(movement) * SimulationConfig.MaxDroneSpeedLimit;
                }

                drone.Position += movement;
                ForceBoundaries(drone);
            }
        }

        /// <summary>
        /// Forces a drone to stay within defined simulation boundaries.
        /// </summary>
        internal void ForceBoundaries(Drone drone)
        {
            drone.Position = new Vector3(
                Math.Clamp(drone.Position.X, SimulationConfig.XMin, SimulationConfig.XMax),
                Math.Clamp(drone.Position.Y, SimulationConfig.YMin, SimulationConfig.YMax),
                Math.Clamp(drone.Position.Z, SimulationConfig.ZMin, SimulationConfig.ZMax)
            );  
        }

        /// <summary>
        /// Simulates wind effect by applying random variations to drone positions.
        /// </summary>
        internal void SimulateWindEffect(IEnumerable<Drone> drones)
        {
            var rnd = new Random();

            foreach (var drone in drones)
            {
                var wind = new Vector3(
                    (float)(rnd.NextDouble() * 1.5 * (rnd.Next(0, 2) == 0 ? -1 : 1)),
                    (float)(rnd.NextDouble() * 1.5 * (rnd.Next(0, 2) == 0 ? -1 : 1)),
                    (float)(rnd.NextDouble() * 1.5 * (rnd.Next(0, 2) == 0 ? -1 : 1))
                );

                drone.Position += wind * SimulationConfig.WindForceFactor;
                ForceBoundaries(drone);
            }
        }

        public Vector3 CalculateCenterOfMass(IEnumerable<Drone> drones) 
        {
            if (!drones.Any())
                return Vector3.Zero;

            var sum = Vector3.Zero;
            int count = 0;

            foreach (var d in drones)
            {
                sum += d.Position;
                count++;
            }

            return sum / count;
        }

        /// <summary>
        /// Moves every drone in the swarm a little bit towards the target.
        /// </summary>
        /// <returns> True if swarm center of mass reaches target. False if don't.</returns>
        public bool MoveSwarmTowardsTarget(IEnumerable<Drone> drones, Vector3 centerOfMass, Vector3 target)
        {
            if (Vector3.Distance(centerOfMass,target) <= SimulationConfig.TargetThreshold) { return true; } //check if we are already at the target
            var direction = target - centerOfMass;

            if (direction != Vector3.Zero)
                direction = Vector3.Normalize(direction);

            foreach (var drone in drones)
            {
                drone.Position += direction * SimulationConfig.SwarmSpeedMultiplier;
                ForceBoundaries(drone);
            }
            return false;
        }
    }
}
