using DroneSim.Core.Configuration;
using DroneSim.Core.Entities;
using System.Numerics;

namespace DroneSim.Core.Services;

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
        var neighbors = new List<(Drone drone, float distanceSquared)>();
        var minSeparationDistanceSquared = SimulationConfig.MinSeparationDistanceSquared;
        var perceptionFactor = SimulationConfig.PerceptionFactor;
        var separationSpeedFactor = SimulationConfig.SeparationSpeedFactor;
        var cohesionSpeedFactor = SimulationConfig.CohesionSpeedFactor;
        var maxDroneSpeedLimit = SimulationConfig.MaxDroneSpeedLimit;

        var centerOfMass = CalculateCenterOfMass(drones);
        var cellSize = SimulationConfig.MinSeparationDistance;
        var grid = new Dictionary<(int, int, int), List<Drone>>();

        foreach (var drone in drones)
        {
            var cell = (
                (int)(drone.Position.X / cellSize),
                (int)(drone.Position.Y / cellSize),
                (int)(drone.Position.Z / cellSize)
            );

            if (!grid.ContainsKey(cell))
                grid[cell] = new List<Drone>();

            grid[cell].Add(drone);
        }

        foreach (var drone in drones)
        {
            var movement = Vector3.Zero;
            neighbors = new List<(Drone, float)>();

            var cell = (
                (int)(drone.Position.X / cellSize),
                (int)(drone.Position.Y / cellSize),
                (int)(drone.Position.Z / cellSize)
            );

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        var neighborCell = (cell.Item1 + dx, cell.Item2 + dy, cell.Item3 + dz);
                        if (!grid.ContainsKey(neighborCell)) continue;

                        foreach (var other in grid[neighborCell])
                        {
                            if (other == drone) continue;
                            float distanceSquared = Vector3.DistanceSquared(drone.Position, other.Position);

                            if (distanceSquared < minSeparationDistanceSquared * perceptionFactor)
                                neighbors.Add((other, distanceSquared));
                        }
                    }
                }
            }

            //Separation
            foreach (var (n, distanceSquared) in neighbors)
            {
                var offset = drone.Position - n.Position;

                if (distanceSquared < minSeparationDistanceSquared && distanceSquared > 0)
                {
                    movement += offset / distanceSquared * separationSpeedFactor;
                }
            }

            //Cohesion
            if (drone.PositionOffset != Vector3.Zero)
            {
                movement += (drone.PositionOffset - drone.Position) * cohesionSpeedFactor;
            }
            else if (neighbors.Count != 0)
            {
                movement += (centerOfMass - drone.Position) * cohesionSpeedFactor;
            }

            //Speed limit
            var speed = movement.Length();
            if (maxDroneSpeedLimit > 0 && speed > maxDroneSpeedLimit)
            {
                movement = Vector3.Normalize(movement) * maxDroneSpeedLimit;
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
                (float)(rnd.NextDouble() * (rnd.Next(0, 2) == 0 ? -1 : 1)),
                (float)(rnd.NextDouble() * (rnd.Next(0, 2) == 0 ? -1 : 1)),
                (float)(rnd.NextDouble() * (rnd.Next(0, 2) == 0 ? -1 : 1))
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
        if (Vector3.Distance(centerOfMass, target) <= SimulationConfig.TargetThreshold) { return true; } //check if we are already at the target
        var direction = target - centerOfMass;

        if (direction != Vector3.Zero)
            direction = Vector3.Normalize(direction);

        foreach (var drone in drones)
        {
            drone.Position += direction * SimulationConfig.SwarmSpeedMultiplier;
            if (drone.PositionOffset != Vector3.Zero)
                drone.PositionOffset += direction * SimulationConfig.SwarmSpeedMultiplier;

            ForceBoundaries(drone);
        }
        return false;
    }
}
