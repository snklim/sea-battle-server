using SeaBattleServer.Game2.Commands;
using System.Collections.Generic;

namespace SeaBattleServer.Game2.States
{
    class GameFinishedState : GameState
    {
        public override List<Cell> Attack(PlayerMoveCommand move)
        {
            return new List<Cell>();
        }
    }
}