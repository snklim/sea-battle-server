using System.Collections.Generic;

namespace SeaBattleServer.Game2.Commands
{
    class PlayerManualMoveCommand : PlayerMoveCommand
    {
        public PlayerManualMoveCommand(Player player, Move move) : base(player)
        {
            Move = move;
        }
    }
}