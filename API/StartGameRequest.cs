namespace SeaBattleServer
{
    public partial class Startup
    {
        public class StartGameRequest
        {
            public string name { get; set; }
            public bool playWithBot { get; set; }
        }
    }
}
