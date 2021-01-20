using System.Collections.Generic;

namespace SeaBattleServer
{
    public class InitData
    {
        public string name { get; set; }
        public int player { get; set; }
        public int nextPlayer { get; set; }
        public Cell[] nextMove { get; set; }
        public List<List<Cell>>[] fields { get; set; }
    }
}