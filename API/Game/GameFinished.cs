using System.Collections.Generic;

namespace SeaBattleServer
{
    public class GameFinished : GameStatus
    {
        Game game;

        public GameFinished(Game game)
        {
            this.game = game;
        }

        public override Changes Move(Shot shot)
        {
            return new Changes
            {
                valid = false,
                x = shot.x,
                y = shot.y,
                cells = new List<Cell>(),
                nextPlayer = game.nextPlayer,
                nextMove = null,
                status = game.status
            };
        }
    }
}