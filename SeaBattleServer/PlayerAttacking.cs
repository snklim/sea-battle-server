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

        public override (bool player, Cell move, bool status, bool valid) Attack(int x, int y, List<Cell> cells)
        {
            if (!player.availableMoves.Any(item => item.x == x && item.y == y))
            {
                return (false, null, false, false);
            }

            var ret = player.AttackInternal(x, y, cells);

            return (ret.player, ret.move, ret.status, true);
        }

        public override void SwitchStatus()
        {
            player.PlayerStatus = new PlayerDefending(player);
        }
    }
}