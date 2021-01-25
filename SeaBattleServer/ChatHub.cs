using Microsoft.AspNetCore.SignalR;
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
            public static Dictionary<string, GameContext> _games = new Dictionary<string, GameContext>();

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
                if (_games.TryGetValue(name, out var ctx) && ctx.Game.status >= 0)
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

            private async Task<bool> StartNew(string name, bool playWithBot)
            {
                var game = new Game() { name = name };
                var playerCtx = new PlayerContext { ConnectionId = Context.ConnectionId, PlayerIndex = 0 };
                var ctx = new GameContext { Game = game, Name = name, PlayWithBot = playWithBot };
                ctx.Players.Add(Context.ConnectionId, playerCtx);
                _games[name] = ctx;

                game.start();

                var initData = ctx.Game.getData(0);

                await Clients.Caller.Started(initData);

                return true;
            }

            public async Task Move(Shot shot)
            {
                try
                {
                    var ctx = _games[shot.name];
                    var changes = ctx.Game.move(shot);

                    foreach (var player in ctx.Players.Values)
                    {
                        await Clients.Client(player.ConnectionId).Moved(changes, player.PlayerIndex == shot.player ? 1 : 0);
                    }
                }
                catch (Exception ex)
                {

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
