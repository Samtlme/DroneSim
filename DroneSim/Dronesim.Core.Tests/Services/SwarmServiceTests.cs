using DroneSim.Core.Services;

namespace DroneSim.Core.Tests.Services;

public class SwarmServiceTests
{
    [Fact]
    public void InitializeSwarm_ShouldCreateExactNumberOfDrones()
    {
        //arrange
        var physicsService = new PhysicsService();
        var commandService = new CommandService();
        var swarmService = new SwarmService(physicsService, commandService);
        int expectedDroneCount = 100;

        //act
        swarmService.InitializeSwarm(expectedDroneCount);
        var droneList = swarmService.GetDroneList;

        //assert
        Assert.Equal(expectedDroneCount, droneList.Count);

        var uniqueIdsCount = droneList.Select(d => d.Id).Distinct().Count();
        Assert.Equal(expectedDroneCount, uniqueIdsCount);
    }

    [Fact]
    public void ClearDroneList_ShouldLeaveSwarmEmpty()
    {
        //arrange
        var physicsService = new PhysicsService();
        var commandService = new CommandService();
        var swarmService = new SwarmService(physicsService, commandService);
        swarmService.InitializeSwarm(100);

        //act
        swarmService.ClearDroneList();

        //assert
        Assert.Empty(swarmService.GetDroneList);
    }
}