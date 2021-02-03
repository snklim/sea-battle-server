using System.Collections.Generic;

namespace SeaBattleServer.Game2.Commands
{
    class PlayerAutoMoveCommand : PlayerMoveCommand
    {
        private readonly Player player;

        public PlayerAutoMoveCommand(Player player)
        {
            this.player = player;
        }

        public override void Execute(List<Cell> affectedCells)
        {
            player.Attack(player.NextMove(), affectedCells);
        }
    }
}