using DroneSim.Core.Services;
using DroneSim.Core.Interfaces;

namespace DroneSim.Core.Tests.Services;

public class FakeCommand : ICommand
{
    public int Priority { get; set; }
    public string Name { get; set; } = "TestCommand";

    public bool WasExecuted { get; private set; } = false;

    public Task<bool> ExecuteAsync()
    {
        WasExecuted = true; 
        return Task.FromResult(true);
    }
}

public class CommandServiceTests
{
    [Fact]
    public async Task EnqueueCommand_WithSamePriority_ShouldOverwriteExistingCommand()
    {
        //arrange
        var service = new CommandService();
        var command1 = new FakeCommand { Priority = 1, Name = "Command1" };
        var command2 = new FakeCommand { Priority = 1, Name = "Command2" };

        //act
        service.EnqueueCommand(command1);
        service.EnqueueCommand(command2);

        await service.TryExecuteCommandAsync();

        //assert
        Assert.False(command1.WasExecuted, "Command1 should be overwritten and never executed.");
        Assert.True(command2.WasExecuted, "Command2 overwritted command1 and was executed.");
    }

    [Fact]
    public async Task TryExecuteCommandAsync_WhenQueueIsEmpty_ShouldReturnSafely()
    {
        //arrange
        var service = new CommandService();

        //act & assert
        var exception = await Record.ExceptionAsync(async () => await service.TryExecuteCommandAsync());

        Assert.Null(exception);
    }
}