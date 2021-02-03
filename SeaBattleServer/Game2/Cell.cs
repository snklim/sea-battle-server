using System;

namespace SeaBattleServer.Game2
{
    class Cell
    {
        public Guid PlayerId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public CellType Type { get; set; } = CellType.Empty;
        public int ShipIndex { get; set; }
    }
}