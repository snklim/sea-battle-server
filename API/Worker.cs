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
            private readonly IHubContext<SeaBattleHub, ISeaBattleClient> _hub;

            public Worker(IHubContext<SeaBattleHub, ISeaBattleClient> hub)
            {
                _hub = hub;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500));

                    foreach (var ctx in SeaBattleHub.games.Where(x => x.Value.playWithBot).Select(x => x.Value))
                    {
                        var changes = ctx.game.GetChanges(new Game2.Commands.PlayerAutoMoveCommand(ctx.game.Player2));

                        if (ctx.Connections.Any())
                            await _hub.Clients.Client(ctx.Connections.First().connection).Moved(changes, 0);
                    }
                }
            }
        }
    }
}
