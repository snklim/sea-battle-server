using System.Collections.Generic;

namespace SeaBattleServer
{
    public class Changes
    {
        public bool valid { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public List<Cell> cells { get; set; } = new List<Cell>();
        public int nextPlayer { get; set; }
        public Cell nextMove { get; set; }
        public int status { get; set; }
    }
}