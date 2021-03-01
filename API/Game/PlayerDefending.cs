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

        public override AttackResponse Attack(int x, int y, List<Cell> cells)
        {
            return new AttackResponse { player = false, move = null, status = false, valid = false };
        }

        public override void SwitchStatus()
        {
            player.PlayerStatus = new PlayerAttacking(player);
        }
    }

    public class AttackResponse
    {
        public bool player { get; set; }
        public Cell move { get; set; }
        public bool status { get; set; }
        public bool valid { get; set; }
    }
}