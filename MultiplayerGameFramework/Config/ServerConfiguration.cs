using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameFramework.Config
{
    public class ServerConfiguration
    {
        public string Name { get; set; }
        public int Port { get; set; }
        public int MaxConnections { get; set; }

        public string LocalAddress { get; set; }

    }
}
