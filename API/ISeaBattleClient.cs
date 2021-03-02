using System.Threading.Tasks;

namespace SeaBattleServer
{
    public partial class Startup
    {
        public interface ISeaBattleClient
        {
            Task Started(InitData message);
            Task Moved(Changes message, int fieldIndex);
        }
    }
}
