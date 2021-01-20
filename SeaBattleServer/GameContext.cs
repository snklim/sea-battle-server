using System.Collections.Generic;

namespace SeaBattleServer
{
    public partial class Startup
    {
        public class GameContext
        {
            public string Name { get; set; }
            public Game Game { get; set; }
            public bool PlayWithBot { get; set; }
            public Dictionary<string, PlayerContext> Players { get; } = new Dictionary<string, PlayerContext>();
        }
    }
}
