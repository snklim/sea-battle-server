using System.Collections.Generic;

namespace SeaBattleServer
{
    public partial class Startup
    {
        public class Context
        {
            internal Game2.Game game { get; set; }
            public List<(string connection, int index)> Connections { get; } = new List<(string connection, int index)>();
            public bool playWithBot { get; set; }
            public List<int> DisconnectedPlayes { get; } = new List<int>();
        }
    }
}
