using System.Collections.Generic;

namespace SeaBattleServer.Game2.Commands
{
    abstract class PlayerMoveCommand
    {
        private readonly Player _player;
        
        protected PlayerMoveCommand(Player player)
        {
            _player = player;
        }

        public void Execute(List<Cell> affectedCells)
        {
            _player.Attack(Move, affectedCells);
        }

        public Move Move { get; protected set; }
    }
}