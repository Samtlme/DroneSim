using System.Numerics;

namespace DroneSim.Core.Entities
{
    public class Drone
    {
        public int Id { get; set; }
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 PositionOffset { get; set; } = Vector3.Zero; //This manages formations

        //TODO methods to get by ref the vectors for performance
    }
}
