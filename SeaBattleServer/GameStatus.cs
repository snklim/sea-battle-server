namespace SeaBattleServer
{
    public abstract class GameStatus
    {
        public abstract Changes Move(Shot shot);
    }
}