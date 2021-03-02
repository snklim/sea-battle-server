using System.Collections.Generic;

namespace SeaBattleServer.Game2.Commands
{
    class PlayerAutoMoveCommand : PlayerMoveCommand
    {
        public PlayerAutoMoveCommand(Player player) : base(player)
        {
            Move = player.NextMove();
        }
    }
}