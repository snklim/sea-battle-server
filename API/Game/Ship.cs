using System.Collections.Generic;

namespace SeaBattleServer
{
    public class Ship
    {
        public int index { get; set; }
        public List<Cell> ship { get; set; }
        public List<Cell> border { get; set; }
        public int length { get; set; }
        public bool processed { get; set; }
    }
}
