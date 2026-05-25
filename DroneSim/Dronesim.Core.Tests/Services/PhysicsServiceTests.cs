using DroneSim.Core.Configuration;
using DroneSim.Core.Entities;
using DroneSim.Core.Services;
using System.Numerics;
using Xunit;

namespace DroneSim.Core.Tests.Services;

public class PhysicsServiceTests
{
    [Fact]
    public void CalculateCenterOfMass_WithMultipleDrones_ShouldReturnCorrectAverage()
    {
        //arrange
        var service = new PhysicsService();
        var drones = new List<Drone>
        {
            new() { Id = 1, Position = new Vector3(0f, 0f, 0f) },
            new() { Id = 2, Position = new Vector3(10f, 20f, 30f) }
        };

        //act
        var centerOfMass = service.CalculateCenterOfMass(drones);

        //assert
        Assert.Equal(5f, centerOfMass.X);
        Assert.Equal(10f, centerOfMass.Y);
        Assert.Equal(15f, centerOfMass.Z);
    }

    [Fact]
    public void CalculateCenterOfMass_WhenNoDrones_ShouldReturnZeroVector()
    {
        //arrange
        var service = new PhysicsService();
        var drones = Enumerable.Empty<Drone>();

        //act
        var centerOfMass = service.CalculateCenterOfMass(drones);

        //assert
        Assert.Equal(Vector3.Zero, centerOfMass);
    }

    [Fact]
    public void MoveSwarmTowardsTarget_WhenAtTargetThreshold_ShouldReturnTrue()
    {
        //arrange
        var service = new PhysicsService();
        var drones = new List<Drone> { new() { Id = 1, Position = new Vector3(0f, 0f, 0f) } };
        var centerOfMass = new Vector3(0f, 0f, 0f);
        var target = new Vector3(0f, 0f, 0f);

        //act
        bool reached = service.MoveSwarmTowardsTarget(drones, centerOfMass, target);

        //assert
        Assert.True(reached);
    }

    [Fact]
    public void ForceBoundaries_WhenDroneIsOutsideBounds_ShouldClampPosition()
    {
        //arrange
        var service = new PhysicsService();

        var outOfBoundsPosition = new Vector3(
            SimulationConfig.XMax + 100f,
            SimulationConfig.YMin - 20f,    //under the floor
            SimulationConfig.ZMax + 100f
        );

        var drone = new Drone { Id = 1, Position = outOfBoundsPosition };

        //act
        service.ForceBoundaries(drone);

        //assert
        Assert.Equal(SimulationConfig.XMax, drone.Position.X);
        Assert.Equal(SimulationConfig.YMin, drone.Position.Y);
        Assert.Equal(SimulationConfig.ZMax, drone.Position.Z);
    }
}