using SeaBattleServer.Game2.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeaBattleServer.Game2.States
{

    abstract class GameState
    {
        abstract public List<Cell> Attack(PlayerMoveCommand move);
    }
}