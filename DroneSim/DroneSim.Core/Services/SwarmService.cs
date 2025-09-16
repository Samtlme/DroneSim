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
                    Id = i + 1,
                    X = rnd.NextDouble() * 10,
                    Y = rnd.NextDouble() * 10,
                    Z = rnd.NextDouble() * 10
                });
            }

        }

        #region Simulation Control
        public void StartSimulation(int refreshRate = 1)    //1 Hz by default
        {
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
            return _drones.FirstOrDefault(d => d.Id == id) ?? new Drone();
        }

        public void UpdateDronePositions()
        {
            //TODO:Update position core logic
        }



    }
}
