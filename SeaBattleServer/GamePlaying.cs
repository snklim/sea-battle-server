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
            var ret = game.moveInternal(shot);

            if(ret.status >= 0)
            {
                game.GameStatus = new GameFinished(game);
            }

            return ret;
        }
    }
}