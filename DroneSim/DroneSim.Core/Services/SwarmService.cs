using DroneSim.Core.Entities;
using System.Numerics;
using Timer = System.Timers.Timer;

namespace DroneSim.Core.Services
{

    public class SwarmService(PhysicsService physics, CommandService commandService)
    {
        private static Timer? _timer;
        private bool _isRunning = false;
        private readonly List<Drone> _drones = [];
        private readonly PhysicsService _physics = physics;
        public event Func<IEnumerable<Drone>, Task>? OnDronesUpdated;
        private readonly CommandService _commandService = commandService;

        public void InitializeSwarm(int droneCount)
        {
            var newDrones = new List<Drone>();

            //Testing purpose only
            var rnd = new Random();
            for (int i = 0; i < droneCount; i++)
            {
                newDrones.Add(new Drone
                {
                    Id = i + 1,
                    Position = new Vector3(
                        (float)rnd.NextDouble() * 10,
                        (float)rnd.NextDouble() * 25 + 15,
                        (float)rnd.NextDouble() * 10)
                });
            }
            _commandService.ClearCommandQueue();

            lock (_drones) 
            {
                ClearDroneList();
                _drones.AddRange(newDrones);
            }
        }

        #region Simulation Control
        public void StartSimulation(int droneCount,int refreshRate)
        {
            InitializeSwarm(droneCount);

            if (_timer == null)
            {
                _timer = new Timer(1000 / refreshRate);
                _timer.Elapsed += async (sender, args) =>
                {
                    if (_isRunning) return;

                    _isRunning = true;

                    try
                    {
                        UpdateDronePositions();
                        if (OnDronesUpdated != null)
                            await OnDronesUpdated.Invoke(GetDroneList);
                    }
                    finally
                    {
                        _isRunning = false;
                    }
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

        public void UpdateDronePositions()
        {
            lock (_drones) {
                //apply boids
                _physics.UpdatePositions(_drones);

                //check for remaining commands
                _ = _commandService.TryExecuteCommandAsync();
            }
        }

        public void ClearDroneList() => _drones.Clear();
        public IReadOnlyList<Drone> GetDroneList => _drones;
        public void AddDrone(Drone drone) => _drones.Add(drone);
        public void RemoveDrone(Drone drone) => _drones.Remove(drone);

    }
}
