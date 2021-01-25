using System.Collections.Generic;
using System.Linq;

namespace SeaBattleServer
{
    public class PlayerAttacking : PlayerStatus
    {
        public Player player;

        public PlayerAttacking(Player player)
        {
            this.player = player;
        }

        public override AttackResponse Attack(int x, int y, List<Cell> cells)
        {
            if (!player.availableMoves.Any(item => item.x == x && item.y == y))
            {
                return new AttackResponse { player = false, move = null, status = false, valid = false };
            }

            var ret = player.AttackInternal(x, y, cells);

            return new AttackResponse { player = ret.player, move = ret.move, status = ret.status, valid = true };
        }

        public override void SwitchStatus()
        {
            player.PlayerStatus = new PlayerDefending(player);
        }
    }
}