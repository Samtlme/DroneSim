using DroneSim.Core.Entities;
using System.Numerics;
using Timer = System.Timers.Timer;

namespace DroneSim.Core.Services
{

    public class SwarmService
    {
        private static Timer? _timer;
        private readonly PhysicsService _physics;
        private readonly CommandService _commandService;
        private readonly List<Drone> _drones = new();
        public event Func<IEnumerable<Drone>, Task>? OnDronesUpdated;

        public SwarmService(PhysicsService physics, CommandService commandService)
        {
            _physics = physics;
            _commandService = commandService;
        }
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
                        (float)rnd.NextDouble() * 25,
                        (float)rnd.NextDouble() * 10)
                });
            }
            _commandService.ClearCommandQueue();
            lock (_drones) 
            {
                _drones.Clear();
                _drones.AddRange(newDrones);
            }
           
        }

        #region Simulation Control
        public void StartSimulation(int droneCount,int refreshRate)    //1 Hz by default
        {
            InitializeSwarm(droneCount);

            if (_timer == null)
            {
                _timer = new Timer(1000 / refreshRate);
                _timer.Elapsed += async (sender, args) =>
                {
                    //TODO: Beware about async calls buildup if processing takes longer than interval, need to test heavy-simulation scenarios at some point
                    UpdateDronePositions();
                    if (OnDronesUpdated != null)
                        await OnDronesUpdated.Invoke(GetDroneList);
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
            }

            //check for remaining commands
            _ = _commandService.TryExecuteCommandAsync();
        }

        public void ClearDroneList() => _drones.Clear();
        public IReadOnlyList<Drone> GetDroneList => _drones;
        public void AddDrone(Drone drone) => _drones.Add(drone);
        public void RemoveDrone(Drone drone) => _drones.Remove(drone);

        public Drone GetDroneById(int id)
        {
            return _drones.FirstOrDefault(d => d.Id == id) ?? new Drone();
        }
    }
}
