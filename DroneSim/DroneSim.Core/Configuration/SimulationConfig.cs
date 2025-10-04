using System.ComponentModel.DataAnnotations;

namespace DroneSim.Core.Configuration
{
    /// <summary>
    /// This class holds configuration settings for the simulation environment.
    /// </summary>
    public static class SimulationConfig
    {
        public static float XMax { get; set; } = 1000;
        public static float XMin => -XMax;
        public static float ZMax { get; set; } = 1000;
        public static float ZMin => -ZMax;
        public static float YMin { get; set; } = 15; //Ground level = 0
        public static float YMax { get; set; } = 19;


        //Simulation speed parameters
        public static float CohesionSpeedFactor { get; set; } = 0.2f; //Fraction(0-1) of the distance to the center of mass to move EACH UPDATE CYCLE. For example, 0.1 means drones will move 10% of the distance to the center of mass each update.
        public static float SeparationSpeedFactor { get; set; } = 0.4f; //Contribution to separation vector on EACH UPDATE CYCLE. Higher values make drones separate faster.
        public static float MaxDroneSpeedLimit { get; set; } = 0; //Maximum drone speed units on EACH UPDATE. 0 Means no limit.
        public static float SwarmSpeedMultiplier { get; set; } = 5; //Maximum swarm speed units on EACH UPDATE. 0 Means no limit.
        public static float PerceptionFactor { get; set; } = 100000; //How many times the "MinSeparationDistance" to define which drones are considered as neighbors. Low values can make drones get separated from the rest.


        //Simulation parameters
        public static float MinSeparationDistance { get; set; } = 50;   //Minimum distance between drones. It multiplies "PerceptionFactor".
        public static float MinSeparationDistanceSquared => MinSeparationDistance * MinSeparationDistance; //Min Separation Squared for performance
        public static float WindForceFactor { get; set; } = 1.5f; //Wind strength factor. 
        public static float TargetThreshold { get; set; } = 30; //Max distantance to target to consider it reached 

    }
}
