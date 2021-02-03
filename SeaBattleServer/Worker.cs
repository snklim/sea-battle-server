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

                    foreach (var ctx in ChatHub.games.Where(x => x.Value.playWithBot).Select(x => x.Value))
                    {
                        var affectedCells = ctx.game.Attack(new Game2.Commands.PlayerAutoMoveCommand(ctx.game.Player2));

                        var changes = new Changes
                        {
                            status = -1,
                            valid = affectedCells.Any(),
                            nextMove = new Cell { },
                            nextPlayer = affectedCells.Any(cell => cell.Type == Game2.CellType.Destroyed) ? 1 : 0,
                            cells = affectedCells.Select(cell => new Cell
                            {
                                type = cell.Type == Game2.CellType.Destroyed ? CellType.Killed : CellType.Missed,
                                x = cell.X,
                                y = cell.Y
                            }).ToList(),
                            x = -1,
                            y = -1
                        };

                        await _hub.Clients.Client(ctx.master).Moved(changes, 0);
                    }
                }
            }
        }
    }
}
