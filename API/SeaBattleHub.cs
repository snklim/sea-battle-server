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
        public class SeaBattleHub : Hub<ISeaBattleClient>
        {
            public static Dictionary<string, Context> games = new Dictionary<string, Context>();

            public async Task Start(StartGameRequest request)
            {
                if (await PlayAgain(request.name, request.playWithBot))
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

            private async Task<bool> PlayAgain(string name, bool playWithBot)
            {
                if (games.TryGetValue(name, out var ctx) && ctx.game.Winner >= 0)
                {
                    ctx.game = GameBuilder.BuildGame(name);

                    await Clients.Client(ctx.Connections.First().connection).Started(ctx.game.GetInitData(0));

                    if (!playWithBot)
                        await Clients.Client(ctx.Connections.Last().connection).Started(ctx.game.GetInitData(1));

                    return true;
                }

                return false;
            }

            private async Task<bool> JoinExists(string name)
            {
                if (games.ContainsKey(name) && games[name].Connections.Count < 2)
                {
                    var ctx = games[name];
                    var playerIndex = 1;

                    if (ctx.DisconnectedPlayes.Any())
                    {
                        playerIndex = ctx.DisconnectedPlayes.First();
                        ctx.DisconnectedPlayes.Remove(playerIndex);
                    }

                    ctx.Connections.Insert(playerIndex, (Context.ConnectionId, playerIndex));

                    await Clients.Caller.Started(ctx.game.GetInitData(playerIndex));

                    return true;
                }

                return false;
            }

            private async Task<bool> StartNew(string name, bool playWithBot)
            {
                if (games.ContainsKey(name)) return false;

                var game = GameBuilder.BuildGame(name);

                games[name] = new Context
                {
                    game = game,
                    Connections = { (Context.ConnectionId, 0) },
                    playWithBot = playWithBot
                };

                await Clients.Client(games[name].Connections.First().connection).Started(games[name].game.GetInitData(0));

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

                var changes = ctx.game.GetChanges(cmd);

                await Clients.Client(ctx.Connections.First().connection).Moved(changes, (shot.player + 1) % 2);

                if (!ctx.playWithBot)
                    await Clients.Client(ctx.Connections.Last().connection).Moved(changes, shot.player);
            }

            public override Task OnDisconnectedAsync(Exception exception)
            {
                if (games.Any(x => x.Value.Connections.Any(x => x.connection == Context.ConnectionId)))
                {
                    var ctx = games.First(x => x.Value.Connections.Any(x => x.connection == Context.ConnectionId));

                    var item = ctx.Value.Connections.Find(x => x.connection == Context.ConnectionId);

                    ctx.Value.Connections.Remove(item);

                    ctx.Value.DisconnectedPlayes.Add(item.index);
                }

                return base.OnDisconnectedAsync(exception);
            }
        }
    }
}
