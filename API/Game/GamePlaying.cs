using System.Collections.Generic;

namespace SeaBattleServer
{
    public class GamePlaying : GameStatus
    {
        Game game;

        public GamePlaying(Game game)
        {
            this.game = game;
        }

        public override Changes Move(Shot shot)
        {
            var changes = new Changes
            {
                valid = false,
                x = shot.x,
                y = shot.y,
                cells = new List<Cell>(),
                nextPlayer = game.nextPlayer,
                nextMove = null,
                status = game.status
            };

            var player = game.players[(shot.player + 1) % 2];

            AttackResponse ret;

            if ((ret = player.Attack(shot.x, shot.y, changes.cells)).valid)
            {
                changes.valid = true;

                changes.nextPlayer = game.nextPlayer = ret.player ? shot.player : (shot.player + 1) % 2;

                if (!ret.player)
                {
                    game.players.ForEach(pl => pl.PlayerStatus.SwitchStatus());
                }

                if (ret.status)
                {
                    game.GameStatus = new GameFinished(game);
                }

                changes.nextMove = ret.move;
                changes.status = game.status = ret.status ? shot.player : game.status;
            }

            return changes;
        }
    }
}