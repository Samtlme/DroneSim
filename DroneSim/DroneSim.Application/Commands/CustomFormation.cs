using DroneSim.Core.Configuration;
using DroneSim.Core.Interfaces;
using DroneSim.Core.Services;
using System.Numerics;

namespace DroneSim.Application.Commands
{
    internal class CustomFormation(SwarmService swarm, List<Vector2> points) : ICommand
    {
        private readonly SwarmService _swarm = swarm;
        private List<Vector2> _points = points;
        public int Priority { get; } = 3;
        public string Name { get; } = "Custom Formation";
        public Task<bool> ExecuteAsync()
        {
            //1) We get the distance we have(points) and the distance we need(drones)
            //2) We scale the drawing to fit the distance we need
            //3) From 1) and 2) we get the factor to scale the drawing
            //4) We get the ideal separation between drones
            //5) For each drone we recreate the trajectory (from the previous segment) with the ideal separation to find the exact segment it belongs to
            //5.1) We interpolate the position in that segment, considering the relative distance between both points (weightInsideSegment)
            //6) Assign positions

            var drones = _swarm.GetDroneList;
            var pointCount = _points.Count;
            var droneCount = drones.Count;

            if (pointCount < 2 || droneCount < 1)
            {
                return Task.FromResult(true);
            }

            var totalDistance = 0f;
            for (int i = 1; i < pointCount; i++)
                totalDistance += Vector2.Distance(_points[i - 1], _points[i]);

            if (totalDistance <= 0f)
            {
                return Task.FromResult(true);
            }

            var distanceNeeded = (droneCount - 1) * SimulationConfig.MinSeparationDistance;
            var adjustmentFactor = distanceNeeded / totalDistance;

            _points = _points.Select(p => p * adjustmentFactor).ToList();  //3

            totalDistance = 0f;
            for (int i = 1; i < pointCount; i++)
                totalDistance += Vector2.Distance(_points[i - 1], _points[i]);

            var step = totalDistance / (droneCount - 1);   //4

            List<Vector2> interpolated = [];
            var accumulated = 0f;
            var currentSegment = 0;

            for (int i = 0; i < droneCount; i++)
            {
                var targetDist = i * step;

                while (currentSegment < pointCount - 2 && accumulated + Vector2.Distance(_points[currentSegment], _points[currentSegment + 1]) < targetDist)  //5
                {
                    accumulated += Vector2.Distance(_points[currentSegment], _points[currentSegment + 1]);
                    currentSegment++;
                }

                var p1 = _points[currentSegment];
                var p2 = _points[currentSegment + 1];
                var segLength = Vector2.Distance(p1, p2);
                var weightInsideSegment = (targetDist - accumulated) / segLength;
                interpolated.Add(Vector2.Lerp(p1, p2, weightInsideSegment));
            }

            for (int i = 0; i < droneCount; i++)
            {
                var pos = interpolated[i];
                drones[i].PositionOffset = new Vector3(pos.X, 20f, pos.Y);
            }

            return Task.FromResult(true);
        }
    }
}
