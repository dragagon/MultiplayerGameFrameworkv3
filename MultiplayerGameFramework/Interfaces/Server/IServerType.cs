﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameFramework.Interfaces.Server
{
    public interface IServerType
    {
        short ServerType { get; }
        string Name { get; }
    }
}
