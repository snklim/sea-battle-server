using System.Collections.Generic;

namespace SeaBattleServer.Game2
{
    class Ship
    {
        public List<Cell> Decks { get; set; }
        public List<Cell> Border { get; set; }
        public int ShipLength { get; set; }
    }
}