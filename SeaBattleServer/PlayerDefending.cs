using System.Collections.Generic;

namespace SeaBattleServer
{
    public class PlayerDefending : PlayerStatus
    {
        public Player player;

        public PlayerDefending(Player player)
        {
            this.player = player;
        }

        public override (bool player, Cell move, bool status, bool valid) Attack(int x, int y, List<Cell> cells)
        {
            return (false, null, false, false);
        }

        public override void SwitchStatus()
        {
            player.PlayerStatus = new PlayerAttacking(player);
        }
    }
}