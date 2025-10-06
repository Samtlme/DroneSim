using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSim.Core.Interfaces
{
    public interface ICommand
    {
        int Priority { get; }
        string Name { get; }
        bool IsCompleted { get; }
        Task<bool> ExecuteAsync();
    }
}
