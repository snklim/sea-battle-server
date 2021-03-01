using SeaBattleServer.Game2.Commands;
using System.Collections.Generic;

namespace SeaBattleServer.Game2.States
{
    class GamePlayingState : GameState
    {
        private readonly Game game;
        private readonly Player player1;
        private readonly Player player2;

        public GamePlayingState(Game game, Player player1, Player player2)
        {
            this.game = game;
            this.player1 = player1;
            this.player2 = player2;
        }

        public override List<Cell> Attack(PlayerMoveCommand cmd)
        {
            var affectedCells = new List<Cell>();

            cmd.Execute(affectedCells);

            if (player1.Field.NumberOfShips == 0 || player2.Field.NumberOfShips == 0)
            {
                game.State = new GameFinishedState();
            }

            if (player1.Field.NumberOfShips == 0)
            {
                game.Winner = 1;
            }

            if (player2.Field.NumberOfShips == 0)
            {
                game.Winner = 0;
            }

            return affectedCells;
        }
    }
}