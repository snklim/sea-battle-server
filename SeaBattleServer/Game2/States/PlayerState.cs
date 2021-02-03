using System.Collections.Generic;

namespace SeaBattleServer.Game2.States
{
    abstract class PlayerState
    {
        public abstract void Attack(Move move, List<Cell> affectedCells);
    }
}