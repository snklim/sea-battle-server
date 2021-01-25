namespace SeaBattleServer
{
    public partial class Startup
    {
        public class PlayerContext
        {
            public int PlayerIndex { get; set; }
            public string ConnectionId { get; set; }
        }
    }
}
