using DroneSim.Core.Configuration;
using DroneSim.Core.Entities;
using DroneSim.Core.Services;

namespace DroneSim.Application.UseCases.Simulation;

public class SimulationManager
{
    private readonly SwarmService _swarmService;

    public SimulationManager(SwarmService swarmService)
    {
        _swarmService = swarmService;
    }

    public void StartSimulation(int droneCount = 1500, int refreshRate = 60)
    {
        _swarmService.StartSimulation(droneCount, refreshRate);
    }

    public void PauseSimulation() => _swarmService.PauseSimulation();

    public void SetConfiguration(SimulationConfigDto newConfig)
    {
        try 
        { 
            SimulationConfig.CohesionSpeedFactor = newConfig.CohesionSpeedFactor ?? SimulationConfig.CohesionSpeedFactor ;
            SimulationConfig.SeparationSpeedFactor = newConfig.SeparationSpeedFactor ?? SimulationConfig.SeparationSpeedFactor;
            SimulationConfig.MaxDroneSpeedLimit = newConfig.MaxDroneSpeedLimit ?? SimulationConfig.MaxDroneSpeedLimit;
            SimulationConfig.SwarmSpeedMultiplier = newConfig.SwarmSpeedMultiplier ?? SimulationConfig.SwarmSpeedMultiplier;
            SimulationConfig.MinSeparationDistance = newConfig.MinSeparationDistance ?? SimulationConfig.MinSeparationDistance;
            SimulationConfig.WindForceFactor = newConfig.WindForceFactor ?? SimulationConfig.WindForceFactor;
            SimulationConfig.TargetThreshold = newConfig.TargetThreshold ?? SimulationConfig.TargetThreshold;

        }
        catch (Exception) 
        {
            throw;
        }
    }
}
