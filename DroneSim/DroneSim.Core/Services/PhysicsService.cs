using DroneSim.Core.Configuration;
using DroneSim.Core.Entities;

namespace DroneSim.Core.Services
{
    public class PhysicsService
    {
        public void UpdatePositions(IEnumerable<Drone> drones)
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
        public void ApplyBoidsRules(IEnumerable<Drone> drones) 
        {
            foreach (var drone in drones)
            {
                var XMovement = 0.0;
                var YMovement = 0.0;
                var ZMovement = 0.0;

                var neighbors = drones.Where(d => d != drone && 
                                             GetDistance(d, drone) < SimulationConfig.MinSeparationDistance * SimulationConfig.PerceptionFactor)
                                      .ToList();

                //Separation
                foreach (var n in neighbors)
                {
                    var XDistance = drone.x - n.x;
                    var YDistance = drone.y - n.y;
                    var ZDistance = drone.z - n.z;
                    var distanceBetween = GetDistance(drone,n);
                    if (distanceBetween < SimulationConfig.MinSeparationDistance)
                    {
                        XMovement += (XDistance / (distanceBetween * distanceBetween)) * SimulationConfig.SeparationSpeedFactor;
                        YMovement += (YDistance / (distanceBetween * distanceBetween)) * SimulationConfig.SeparationSpeedFactor;
                        ZMovement += (ZDistance / (distanceBetween * distanceBetween)) * SimulationConfig.SeparationSpeedFactor;
                    }
                }

                //Cohesion
                if (neighbors.Any())
                {
                    var centerX = neighbors.Average(n => n.x);
                    var centerY = neighbors.Average(n => n.y);
                    var centerZ = neighbors.Average(n => n.z);

                    XMovement += (centerX - drone.x) * SimulationConfig.CohesionSpeedFactor;
                    YMovement += (centerY - drone.y) * SimulationConfig.CohesionSpeedFactor;
                    ZMovement += (centerZ - drone.z) * SimulationConfig.CohesionSpeedFactor;
                }

                //Speed limit
                var speed = Math.Sqrt(XMovement * XMovement + YMovement * YMovement + ZMovement * ZMovement);
                if (SimulationConfig.MaxDroneSpeedLimit > 0 && speed > SimulationConfig.MaxDroneSpeedLimit)
                {
                    XMovement = XMovement / speed * SimulationConfig.MaxDroneSpeedLimit;
                    YMovement = YMovement / speed * SimulationConfig.MaxDroneSpeedLimit;
                    ZMovement = ZMovement / speed * SimulationConfig.MaxDroneSpeedLimit;
                }

                //Force drones to stay within limits
                drone.x = Math.Clamp(drone.x + XMovement, SimulationConfig.XMin, SimulationConfig.XMax);
                drone.y = Math.Clamp(drone.y + YMovement, SimulationConfig.YMin, SimulationConfig.YMax);
                drone.z = Math.Clamp(drone.z + ZMovement, SimulationConfig.ZMin, SimulationConfig.ZMax);
            }
        }


        /// <summary>
        /// Returns the distance between two drones in 3D space.
        /// </summary>
        /// <param name="a">Drone A</param>
        /// <param name="b">Drone B</param>
        /// <returns>Distance value</returns>
        public double GetDistance(Drone a, Drone b)
        {
            var dx = a.x - b.x;
            var dy = a.y - b.y;
            var dz = a.z - b.z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public void SimulateWindEffect(IEnumerable<Drone> drones)
        {
            var rnd = new Random();
            foreach (var drone in drones)
            {
                drone.x += (rnd.NextDouble() * 1.5) * (rnd.Next(0, 2) == 0 ? -1 : 1) * SimulationConfig.WindForceFactor;
                drone.y += (rnd.NextDouble() * 1.5) * (rnd.Next(0, 2) == 0 ? -1 : 1) * SimulationConfig.WindForceFactor;
                drone.z += (rnd.NextDouble() * 1.5) * (rnd.Next(0, 2) == 0 ? -1 : 1) * SimulationConfig.WindForceFactor;
            }
        }

    }
}
