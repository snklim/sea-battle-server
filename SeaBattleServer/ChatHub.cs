using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SeaBattleServer
{
    public partial class Startup
    {
        public class ChatHub : Hub<IChatClient>
        {
            private static Dictionary<string, GameContext> _games = new Dictionary<string, GameContext>();

            public async Task SendMessage(ChatMessage message)
            {
                await Clients.All.ReceiveMessage(message);
            }

            public async Task Start(string name)
            {
                if (await PlayAgain(name))
                {
                    Console.WriteLine("Play again");
                }
                else if (await JoinExists(name))
                {
                    Console.WriteLine("Join exists");
                }
                else if (await StartNew(name))
                {
                    Console.WriteLine("Start new");
                }
            }

            private async Task<bool> PlayAgain(string name)
            {
                if (_games.TryGetValue(name, out var ctx) && ctx.Players.Count == 2 && ctx.Game.status >= 0)
                {
                    ctx.Game.start();

                    foreach (var player in ctx.Players.Values)
                        await Clients.Client(player.ConnectionId).Started(ctx.Game.getData(player.PlayerIndex));

                    return true;
                }

                return false;
            }

            private async Task<bool> JoinExists(string name)
            {
                if (_games.ContainsKey(name))
                {
                    var ctx = _games[name];
                    var playerIndex = ctx.Players.Values.Any(x => x.PlayerIndex == 0) ? 1 : 0;
                    var playerCtx = new PlayerContext { ConnectionId = Context.ConnectionId, PlayerIndex = playerIndex };
                    ctx.Players.Add(Context.ConnectionId, playerCtx);

                    await Clients.Caller.Started(ctx.Game.getData(playerIndex));

                    return true;
                }

                return false;
            }

            private async Task<bool> StartNew(string name)
            {
                var game = new Game() { name = name };
                var playerCtx = new PlayerContext { ConnectionId = Context.ConnectionId, PlayerIndex = 0 };
                var ctx = new GameContext { Game = game, Name = name, };
                ctx.Players.Add(Context.ConnectionId, playerCtx);
                _games[name] = ctx;

                game.start();

                await Clients.Caller.Started(ctx.Game.getData(0));

                return true;
            }

            public async Task Move(Shot shot)
            {
                var ctx = _games[shot.name];
                var changes = ctx.Game.move(shot);

                foreach (var player in ctx.Players.Values)
                {
                    await Clients.Client(player.ConnectionId).Moved(changes, player.PlayerIndex == shot.player ? 1 : 0);
                }
            }

            public override Task OnDisconnectedAsync(Exception exception)
            {
                if (_games.Any(x => x.Value.Players.ContainsKey(Context.ConnectionId)))
                {
                    var ctx = _games.First(x => x.Value.Players.ContainsKey(Context.ConnectionId));

                    ctx.Value.Players.Remove(Context.ConnectionId);
                }

                return base.OnDisconnectedAsync(exception);
            }
        }
    }
}
