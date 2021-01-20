using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace SeaBattleServer
{
    public partial class Startup
    {
        public class Worker : BackgroundService
        {
            private readonly IHubContext<ChatHub, IChatClient> _hub;

            public Worker(IHubContext<ChatHub, IChatClient> hub)
            {
                _hub = hub;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500));

                    foreach (var kv in ChatHub._games.Where(x => x.Value.PlayWithBot && x.Value.Game.status == -1 && x.Value.Game.nextPlayer == 1))
                    {
                        var nextMove = kv.Value.Game.players[0].generateNextMoveFn();

                        var ctx = ChatHub._games[kv.Key];
                        var changes = ctx.Game.move(new Shot
                        {
                            name = kv.Key,
                            player = 1,
                            x = nextMove.x,
                            y = nextMove.y
                        });

                        foreach (var player in ctx.Players.Values)
                        {
                            await _hub.Clients.Client(player.ConnectionId).Moved(changes, 0);
                        }
                    }
                }
            }
        }
    }
}
