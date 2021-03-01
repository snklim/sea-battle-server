using System.Collections.Generic;

namespace SeaBattleServer.Game2.Commands
{
    abstract class PlayerMoveCommand
    {
        abstract public void Execute(List<Cell> affectedCells);
    }
}