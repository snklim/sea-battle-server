using System.Collections.Generic;

namespace SeaBattleServer.Game2.States
{
    class PlayerAttackingState : PlayerState
    {
        private readonly Player player;
        private readonly Player opponent;

        public PlayerAttackingState(Player player, Player opponent)
        {
            this.player = player;
            this.opponent = opponent;
        }

        public override void Attack(Move move, List<Cell> affectedCells)
        {
            if (!opponent.Field.Attack(move, affectedCells))
            {
                player.State = new PlayerDefendingState();
                opponent.State = new PlayerAttackingState(opponent, player);
            }

            affectedCells.ForEach(cell => cell.PlayerId = opponent.PlayerId);
        }
    }
}