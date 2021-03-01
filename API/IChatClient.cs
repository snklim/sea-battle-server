using System.Threading.Tasks;

namespace SeaBattleServer
{
    public partial class Startup
    {
        public interface IChatClient
        {
            Task ReceiveMessage(ChatMessage message);
            Task Started(InitData message);
            Task Moved(Changes message, int fieldIndex);
        }
    }
}
