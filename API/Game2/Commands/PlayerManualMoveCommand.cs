using System.Collections.Generic;

namespace SeaBattleServer.Game2.Commands
{
    class PlayerManualMoveCommand : PlayerMoveCommand
    {
        private readonly Player player;
        private readonly Move move;

        public PlayerManualMoveCommand(Player player, Move move)
        {
            this.player = player;
            this.move = move;
        }

        public override void Execute(List<Cell> affectedCells)
        {
            player.Attack(move, affectedCells);
        }
    }
}