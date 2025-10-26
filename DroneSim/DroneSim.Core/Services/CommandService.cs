using DroneSim.Core.Configuration;
using DroneSim.Core.Interfaces;

namespace DroneSim.Core.Services;

public class CommandService
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private PriorityQueue<ICommand, int> _commandQueue = new();

    /// <summary>
    /// Enqueues a new command into the command queue.
    /// </summary>
    /// <returns>True if it was successfully enqueued. Otherwise, false.</returns>
    public bool EnqueueCommand(ICommand command)
    {
        lock (_commandQueue)
        {
            if (_commandQueue.Count >= SimulationConfig.MaxCommandsInQueue)
                return false;

            var preservedCommands = new List<(ICommand Command, int Priority)>();

            //Only one command per priority. Overwrite if exists
            while (_commandQueue.TryDequeue(out var existing, out var priority))
            {
                if (priority == command.Priority)
                    continue;

                preservedCommands.Add((existing, priority));
            }

            foreach (var (cmd, prio) in preservedCommands)
                _commandQueue.Enqueue(cmd, prio);

            _commandQueue.Enqueue(command, command.Priority);
            return true;
        }
    }

    public void ClearCommandQueue()
    {
        lock (_commandQueue)
        {
            _commandQueue.Clear();
        }
    }

    public async Task TryExecuteCommandAsync()
    {
        if (!await _semaphore.WaitAsync(0))
            return;

        try
        {
            ICommand? command = null;
            lock (_commandQueue)
            {
                if (_commandQueue.Count > 0)
                    command = _commandQueue.Dequeue();
            }

            if (command == null)
                return;

            bool finished = false;
            try
            {
                finished = await command.ExecuteAsync();
            }
            catch (Exception)
            {
                //log in a future or throw to an upper level
            }

            if (!finished)
            {
                lock (_commandQueue)
                {
                    _commandQueue.Enqueue(command, command.Priority);
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

}
