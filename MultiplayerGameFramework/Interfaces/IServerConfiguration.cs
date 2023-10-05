using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameFramework.Interfaces
{
    public interface IServerConfiguration
    {
        string Name { get; }
        int Port { get; }
        int MaxConnections { get; }

    }
}
