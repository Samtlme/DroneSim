using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSim.Core.Entities
{
    public class SimulationConfigDto
    {
        //Simulation speed parameters
        public float? CohesionSpeedFactor { get; set; }
        public float? SeparationSpeedFactor { get; set; }
        public float? MaxDroneSpeedLimit { get; set; }
        public float? SwarmSpeedMultiplier { get; set; }

        //Simulation parameters
        public float? MinSeparationDistance { get; set; }
        public float? WindForceFactor { get; set; }
        public float? TargetThreshold { get; set; }
    }
}
