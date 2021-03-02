using SeaBattleServer.Game2.States;

namespace SeaBattleServer.Game2.Builders
{
    static class GameBuilder
    {
        static public Game BuildGame(string name)
        {
            var player1 = PlayerBuilder.BuildPlayer();
            var player2 = PlayerBuilder.BuildPlayer();

            player1.State = new PlayerAttackingState(player1, player2);
            player2.State = new PlayerDefendingState();

            var game = new Game
            {
                Name = name,
                Player1 = player1,
                Player2 = player2
            };

            game.State = new GamePlayingState(game, player1, player2);

            return game;
        }
    }
}