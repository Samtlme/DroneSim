namespace DroneSim.Core.Interfaces;

public interface ICommand
{
    int Priority { get; }
    string Name { get; }
    Task<bool> ExecuteAsync();
}
