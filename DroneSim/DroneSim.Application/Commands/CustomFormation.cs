using DroneSim.Core.Configuration;
using DroneSim.Core.Interfaces;
using DroneSim.Core.Services;
using System.Numerics;

namespace DroneSim.Application.Commands;

internal class CustomFormation(SwarmService swarm, List<List<Vector2>> pointListVectors) : ICommand
{
    private readonly SwarmService _swarm = swarm;
    private List<List<Vector2>> _pointListVectors = pointListVectors;
    public int Priority { get; } = 3;
    public string Name { get; } = "Custom Formation";
    public Task<bool> ExecuteAsync()
    {
        //1) We get the distance we have(points) and the distance we need(drones)
        //2) We scale the drawing to fit the distance we need (based on min separation)
        //3) From 1) and 2) we get the factor to scale each drawing
        //4) We get the total distance and each segment length
        //5) Distribute frones on each segment
        //5.1) If we remain with extra drones, fit them on the first segment
        //5.2) We get the ideal step for each segment and its number of drones
        //5.3) Interpolate position on each segment
        //6) Assign positions

        var drones = _swarm.GetDroneList;
        var droneCount = drones.Count;

        if (_pointListVectors == null || _pointListVectors.Count == 0 || droneCount < 1)
            return Task.FromResult(true);

        float totalDistance = 0f;
        var strokeDistances = new List<float>();

        foreach (var stroke in _pointListVectors)
        {
            float strokeDist = 0f;
            for (int i = 1; i < stroke.Count; i++)
                strokeDist += Vector2.Distance(stroke[i - 1], stroke[i]);

            strokeDistances.Add(strokeDist);
            totalDistance += strokeDist;
        }

        if (totalDistance <= 0f)
            return Task.FromResult(true);

        var distanceNeeded = (droneCount - 1) * SimulationConfig.MinSeparationDistance;
        var adjustmentFactor = distanceNeeded / totalDistance;

        for (int i = 0; i < _pointListVectors.Count; i++) 
        {
            _pointListVectors[i] = _pointListVectors[i].Select(x => x * adjustmentFactor).ToList(); //3
        }

        totalDistance = 0f;

        for (int i = 0; i < _pointListVectors.Count; i++)
        {
            float strokeDist = 0f;
            for (int j = 1; j < _pointListVectors[i].Count; j++)
                strokeDist += Vector2.Distance(_pointListVectors[i][j - 1], _pointListVectors[i][j]);
            strokeDistances[i] = strokeDist;    //4
            totalDistance += strokeDist;
        }

        var dronesPerStroke = new List<int>();
        int assigned = 0;

        for (int i = 0; i < _pointListVectors.Count; i++)
        {
            int n = (int)Math.Round((strokeDistances[i] / totalDistance) * droneCount); //5
            dronesPerStroke.Add(n);
            assigned += n;
        }

        if (assigned < droneCount) 
        {
            dronesPerStroke[0] += droneCount - assigned;   //5.1
        }

        var interpolatedPositions = interpolatePositions(dronesPerStroke, strokeDistances, _pointListVectors); //5.2 and 5.3

        var flightHeight = SimulationConfig.YMin + 15f;

        for (int i = 0; i < drones.Count && i < interpolatedPositions.Count; i++)
        {
            var pos = interpolatedPositions[i];
            drones[i].PositionOffset = new Vector3(pos.X, flightHeight, pos.Y);
        }

        return Task.FromResult(true);
    }

    private List<Vector2> interpolatePositions(List<int> dronesPerStroke, List<float> strokeDistances, List<List<Vector2>> strokeList) 
    {
        var positions = new List<Vector2>();
        for (int i = 0; i < strokeList.Count; i++)
        {
            //(*)remove consecutive points with identical coordinates to prevent zero length segments (divide by 0 later)
            strokeList[i] = strokeList[i].Where((p, index) => index == 0 || p.X != strokeList[i][index - 1].X || p.Y != strokeList[i][index - 1].Y).ToList();

            var stroke = strokeList[i];
            if (stroke.Count < 2 || dronesPerStroke[i] < 1)
                continue;

            var strokeLength = strokeDistances[i];
            float step = dronesPerStroke[i] > 1 ? strokeLength / (dronesPerStroke[i] - 1) : 0f;

            float accumulated = 0f;
            int currentSegment = 0;

            for (int j = 0; j < dronesPerStroke[i]; j++)
            {
                float targetDist = j * step;

                while (currentSegment < stroke.Count - 2 &&
                       accumulated + Vector2.Distance(stroke[currentSegment], stroke[currentSegment + 1]) < targetDist)
                {
                    accumulated += Vector2.Distance(stroke[currentSegment], stroke[currentSegment + 1]);
                    currentSegment++;
                }

                var p1 = stroke[currentSegment];
                var p2 = stroke[currentSegment + 1];
                var segLength = Vector2.Distance(p1, p2);

                if (segLength == 0f) //avoid divide by 0 if a segment is too short, might happen in some scenarios EVEN after filtering (*)
                {
                    positions.Add(p1);
                    continue;
                }

                var weightInsideSegment = (targetDist - accumulated) / segLength;

                positions.Add(Vector2.Lerp(p1, p2, weightInsideSegment));
            }
        }
        return positions;
    }
}
