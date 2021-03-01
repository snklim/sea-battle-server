using SeaBattleServer.Game2.Commands;
using SeaBattleServer.Game2.States;
using System.Collections.Generic;

namespace SeaBattleServer.Game2
{
    class Game
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public GameState State { get; set; }
        public int Winner { get; set; } = -1;
        public int NextPlayer => Player1.State is PlayerAttackingState ? 0 : 1;

        public List<Cell> Attack(PlayerMoveCommand move)
        {
            return State.Attack(move);
        }
    }
}