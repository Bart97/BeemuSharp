using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EOHax.Programs.EOSERV.Events.PlayerEvents
{
    class WalkEventArgs
    {
        public WalkEventArgs(IServer _server, Character _character)
        {
            server = _server;
            character = _character;
        }
        IServer server;
        Character character;
    }
}
