using DroneSim.Core.Entities;
using Timer = System.Timers.Timer;

namespace DroneSim.Core.Services
{

    public class SwarmService
    {
        private readonly List<Drone> _drones = new();
        private static Timer? _timer;
        public event Func<IEnumerable<Drone>, Task>? OnDronesUpdated;
        
        public void InitializeSwarm(int droneCount = 5) //initialize with 5 drones by default
        {
            _drones.Clear();

            //Testing purpose only
            var rnd = new Random();    
            for (int i = 0; i < droneCount; i++)
            {
                _drones.Add(new Drone
                {
                    id = i + 1,
                    x = rnd.NextDouble() * 10,
                    y = rnd.NextDouble() * 25,
                    z = rnd.NextDouble() * 10
                });
            }

        }

        #region Simulation Control
        public void StartSimulation(int refreshRate = 1)    //1 Hz by default
        {
            InitializeSwarm();

            if (_timer == null)
            {
                _timer = new Timer(1000 * refreshRate);
                _timer.Elapsed += async (sender, args) =>
                {
                    //TODO: Beware about async calls buildup if processing takes longer than interval, need to test heavy-simulation scenarios at some point
                    UpdateDronePositions();
                    if (OnDronesUpdated != null)
                        await OnDronesUpdated.Invoke(_drones);
                };
                _timer.AutoReset = true;
            }
            _timer.Start();
        }

        public void PauseSimulation()
        {
            _timer?.Stop();
        }

        #endregion


        public IEnumerable<Drone> GetAllDronePositions() => _drones;

        public Drone GetDroneById(int id)
        {
            return _drones.FirstOrDefault(d => d.id == id) ?? new Drone();
        }

        public void UpdateDronePositions()
        {
            //TODO:Update position core logic
            var rnd = new Random();
            foreach (var drone in _drones)
            {
                drone.x += (rnd.NextDouble() * 1.5) * (rnd.Next(0, 2) == 0 ? -1 : 1);
                drone.y += (rnd.NextDouble() * 1.5) * (rnd.Next(0, 2) == 0 ? -1 : 1);
                drone.z += (rnd.NextDouble() * 1.5) * (rnd.Next(0, 2) == 0 ? -1 : 1);
            }
        }



    }
}
