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
                public List<string> Connections { get; } = new List<string>();
                public bool playWithBot { get; set; }
                public List<int> DisconnectedPlayes { get; } = new List<int>();
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
                if (games.TryGetValue(name, out var ctx) && ctx.game.Winner >= 0)
                {
                    ctx.game = GameBuilder.BuildGame();

                    await Clients.Client(ctx.Connections.First()).Started(new InitData
                    {
                        name = name,
                        player = 0,
                        nextPlayer = 0,
                        nextMove = new Cell[] { },
                        fields = new[]
                    {
                        Enumerable.Range(0, 10)
                            .Select(x => Enumerable.Range(0, 10).Select(y => new Cell{x=x,y=y,type= ctx.game.Player1.Field.GetState(x, y) == Game2.CellType.Deck ? CellType.Deck : CellType.Empty }).ToList())
                            .ToList(),
                        Enumerable.Range(0, 10)
                            .Select(x => Enumerable.Range(0, 10).Select(y => new Cell{ x=x,y=y,type= CellType.Empty }).ToList())
                            .ToList()
                    }
                    });

                    await Clients.Client(ctx.Connections.Last()).Started(new InitData
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

            private async Task<bool> JoinExists(string name)
            {
                if (games.ContainsKey(name))
                {
                    var ctx = games[name];

                    var player1 = ctx.game.Player2;
                    var player2 = ctx.game.Player1;
                    var playerIndex = 1;

                    if (ctx.DisconnectedPlayes.Any())
                    {
                        playerIndex = ctx.DisconnectedPlayes.First();
                        ctx.DisconnectedPlayes.Remove(playerIndex);
                        Console.WriteLine(playerIndex);
                        if (playerIndex == 0)
                        {
                            player1 = ctx.game.Player1;
                            player2 = ctx.game.Player2;
                        };
                    }

                    ctx.Connections.Insert(playerIndex, Context.ConnectionId);

                    await Clients.Caller.Started(new InitData
                    {
                        name = name,
                        player = playerIndex,
                        nextPlayer = ctx.game.NextPlayer,
                        nextMove = new Cell[] { },
                        fields = new[]
                        {
                        Enumerable.Range(0, 10)
                            .Select(x => Enumerable.Range(0, 10).Select(y => new Cell
                            {
                                x=x,
                                y=y,
                                type=

                                player1.Field.GetState(x, y) switch
                            {
                                Game2.CellType.Deck => CellType.Deck,
                                Game2.CellType.Destroyed => CellType.Killed,
                                Game2.CellType.Missed => CellType.Missed,
                                _ => CellType.Empty
                            }


                                 })
                            .ToList())
                            .ToList(),
                        Enumerable.Range(0, 10)
                            .Select(x => Enumerable.Range(0, 10).Select(y => new Cell{ x=x,y=y,type=

                            player2.Field.GetState(x, y) switch
                            {
                                Game2.CellType.Destroyed => CellType.Killed,
                                Game2.CellType.Missed => CellType.Missed,
                                _ => CellType.Empty
                            }


                             }).ToList())
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
                    Connections = { Context.ConnectionId },
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
                    status = ctx.game.Winner,
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

                await Clients.Client(ctx.Connections.First()).Moved(changes, (shot.player + 1) % 2);
                if (!ctx.playWithBot)
                    await Clients.Client(ctx.Connections.Last()).Moved(changes, shot.player);
            }

            public override Task OnDisconnectedAsync(Exception exception)
            {
                if (games.Any(x => x.Value.Connections.Any(x => x == Context.ConnectionId)))
                {
                    var ctx = games.First(x => x.Value.Connections.Any(x => x == Context.ConnectionId));

                    var playerIndex = ctx.Value.Connections.IndexOf(Context.ConnectionId);

                    ctx.Value.Connections.RemoveAt(playerIndex);

                    ctx.Value.DisconnectedPlayes.Add(playerIndex);
                }

                return base.OnDisconnectedAsync(exception);
            }
        }
    }
}
