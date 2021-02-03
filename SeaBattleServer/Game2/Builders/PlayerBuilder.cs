namespace SeaBattleServer.Game2.Builders
{
    static class PlayerBuilder
    {
        static public Player BuildPlayer()
        {
            var builder = new FieldBuilder();

            var field = builder
                .BuildField(10, 10)
                .BuildShip(4)
                .BuildShip(3)
                .BuildShip(3)
                .BuildShip(2)
                .BuildShip(2)
                .BuildShip(2)
                .BuildShip(1)
                .BuildShip(1)
                .BuildShip(1)
                .BuildShip(1)
                .GetField();

            var player = new Player
            {
                Field = field
            };

            return player;
        }
    }
}