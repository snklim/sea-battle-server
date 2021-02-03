using Microsoft.AspNetCore.SignalR;
using SeaBattleServer.Game2.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeaBattleServer
{
    public partial class Startup
    {
        public class ChatHub : Hub<IChatClient>
        {
            public class ctx
            {
                internal Game2.Game game { get; set; }
                public string master { get; set; } 
                public string slave { get; set; }
                public bool playWithBot { get; set; }
            }

            public static Dictionary<string, ctx> games = new Dictionary<string, ctx>();

            public async Task SendMessage(ChatMessage message)
            {
                await Clients.All.ReceiveMessage(message);
            }

            public async Task Start(StartGameRequest request)
            {
                if (await PlayAgain(request.name))
                {
                    Console.WriteLine("Play again");
                }
                else if (await JoinExists(request.name))
                {
                    Console.WriteLine("Join exists");
                }
                else if (await StartNew(request.name, request.playWithBot))
                {
                    Console.WriteLine("Start new");
                }
            }

            private async Task<bool> PlayAgain(string name)
            {
                //if (_games.TryGetValue(name, out var ctx) && ctx.Game.status >= 0)
                //{
                //    ctx.Game.start();

                //    foreach (var player in ctx.Players.Values)
                //        await Clients.Client(player.ConnectionId).Started(ctx.Game.getData(player.PlayerIndex));

                //    return true;
                //}

                return false;
            }

            private async Task<bool> JoinExists(string name)
            {
                if (games.ContainsKey(name))
                {
                    var ctx = games[name];

                    ctx.slave = Context.ConnectionId;

                    await Clients.Caller.Started(new InitData
                    {
                        name = name,
                        player = 1,
                        nextPlayer = 0,
                        nextMove = new Cell[] { },
                        fields = new[]
                        {
                        Enumerable.Range(0, 10)
                            .Select(x => Enumerable.Range(0, 10).Select(y => new Cell
                            {
                                x=x,
                                y=y,
                                type= ctx.game.Player2.Field.GetState(x, y) == Game2.CellType.Deck ? CellType.Deck : CellType.Empty })
                            .ToList())
                            .ToList(),
                        Enumerable.Range(0, 10)
                            .Select(x => Enumerable.Range(0, 10).Select(y => new Cell{ x=x,y=y,type= CellType.Empty }).ToList())
                            .ToList()
                    }
                    });

                    return true;
                }

                return false;
            }

            private async Task<bool> StartNew(string name, bool playWithBot)
            {
                var game = GameBuilder.BuildGame();

                games[name] = new ctx
                {
                    game = game,
                    master = Context.ConnectionId,
                    playWithBot = playWithBot
                };

                await Clients.Caller.Started(new InitData
                {
                    name = name,
                    player = 0,
                    nextPlayer = 0,
                    nextMove = new Cell[] { },
                    fields = new[]
                    {
                        Enumerable.Range(0, 10)
                            .Select(x => Enumerable.Range(0, 10).Select(y => new Cell{x=x,y=y,type= game.Player1.Field.GetState(x, y) == Game2.CellType.Deck ? CellType.Deck : CellType.Empty }).ToList())
                            .ToList(),
                        Enumerable.Range(0, 10)
                            .Select(x => Enumerable.Range(0, 10).Select(y => new Cell{ x=x,y=y,type= CellType.Empty }).ToList())
                            .ToList()
                    }
                });

                return true;
            }

            public async Task Move(Shot shot)
            {
                var ctx = games[shot.name];

                var cmd = new Game2.Commands.PlayerManualMoveCommand(shot.player == 0 ? ctx.game.Player1 : ctx.game.Player2, new Game2.Move
                {
                    X = shot.x,
                    Y = shot.y
                });

                var affectedCells = ctx.game.Attack(cmd);

                var changes = new Changes
                {
                    status = -1,
                    valid = affectedCells.Any(),
                    nextMove = new Cell { },
                    nextPlayer = affectedCells.Any(cell => cell.Type == Game2.CellType.Destroyed) ? shot.player : (shot.player + 1) % 2,
                    cells = affectedCells.Select(cell => new Cell
                    {
                        type = cell.Type == Game2.CellType.Destroyed ? CellType.Killed : CellType.Missed,
                        x = cell.X,
                        y = cell.Y
                    }).ToList(),
                    x = shot.x,
                    y = shot.y
                };

                await Clients.Client(ctx.master).Moved(changes, (shot.player + 1) % 2);
                if (!ctx.playWithBot)
                    await Clients.Client(ctx.slave).Moved(changes, shot.player);
            }

            public override Task OnDisconnectedAsync(Exception exception)
            {
                //if (games.Any(x => x.Value.master == Context.ConnectionId || x.Value.slave == Context.ConnectionId))
                //{
                //    var ctx = _games.First(x => x.Value.Players.ContainsKey(Context.ConnectionId));

                //    ctx.Value.Players.Remove(Context.ConnectionId);
                //}

                return base.OnDisconnectedAsync(exception);
            }
        }
    }
}
