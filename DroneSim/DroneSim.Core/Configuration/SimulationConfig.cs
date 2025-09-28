namespace DroneSim.Core.Configuration
{
    /// <summary>
    /// This class holds configuration settings for the simulation environment.
    /// </summary>
    public static class SimulationConfig
    {
        public static double XMax { get; set; } = 1000;
        public static double XMin => -XMax;
        public static double ZMax { get; set; } = 1000;
        public static double ZMin => -ZMax;
        public static double YMin { get; set; } = 15; //Ground level = 0
        public static double YMax { get; set; } = 19;


        //Simulation speed parameters
        public static double CohesionSpeedFactor { get; set; } = 0.2; //Fraction(0-1) of the distance to the center of mass to move EACH UPDATE CYCLE. For example, 0.1 means drones will move 10% of the distance to the center of mass each update.
        public static double SeparationSpeedFactor { get; set; } = 0.4; //Contribution to separation vector on EACH UPDATE CYCLE. Higher values make drones separate faster.
        public static double MaxDroneSpeedLimit { get; set; } = 0; //Maximum drone speed units on EACH UPDATE. 0 Means no limit.
        public static double PerceptionFactor { get; set; } = 100000; //How many times the "MinSeparationDistance" to define which drones are considered as neighbors. Low values can make drones get separated from the rest.


        //Simulation parameters
        public static double MinSeparationDistance { get; set; } = 50;   //Minimum distance between drones. It multiplies "PerceptionFactor".
        public static double MinSeparationDistanceSquared => MinSeparationDistance * MinSeparationDistance; //Min Separation Squared for performance
        public static double WindForceFactor { get; set; } = 1.5; //Wind strength factor. 

    }
}
