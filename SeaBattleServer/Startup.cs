using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SeaBattleServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();

            services.AddCors(options =>
            {
                options.AddPolicy("ClientPermission", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("http://localhost:3000")
                        .AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("ClientPermission");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapHub<ChatHub>("/hubs/chat");
            });
        }

        public class ChatHub : Hub<IChatClient>
        {
            private static Dictionary<string, Tuple<Dictionary<string, int>, Game>> _games = new Dictionary<string, Tuple<Dictionary<string, int>, Game>>();

            public async Task SendMessage(ChatMessage message)
            {
                await Clients.All.ReceiveMessage(message);
            }

            public async Task Start(string name)
            {
                if (_games.ContainsKey(name) && _games[name].Item1.Count == 2 && _games[name].Item2.status >= 0)
                {
                    _games[name].Item2.start();

                    foreach(var kv in _games[name].Item1)
                    {
                        await Clients.Client(kv.Key).Started(_games[name].Item2.getData(kv.Value));
                    }
                }
                else
                {
                    if (_games.ContainsKey(name))
                    {
                        var player = _games[name].Item1.Any(x => x.Value == 0) ? 1 : 0;
                        _games[name].Item1.Add(Context.ConnectionId, player);
                    }
                    else
                    {
                        var game = new Game() { name = name };
                        game.start();
                        _games[name] = Tuple.Create(new Dictionary<string, int>() { { Context.ConnectionId, 0 } }, game);
                    }

                    var ctx = _games[name];

                    await Clients.Caller.Started(ctx.Item2.getData(ctx.Item1[Context.ConnectionId]));
                }
            }

            public async Task Move(Shot shot)
            {
                try
                {
                    var ctx = _games[shot.name];
                    var changes = ctx.Item2.move(shot);

                    foreach (var kv in ctx.Item1)
                    {
                        changes.fieldIndex = kv.Value == shot.player ? 1 : 0;
                        await Clients.Client(kv.Key).Moved(changes);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            public override Task OnDisconnectedAsync(Exception exception)
            {
                if (_games.Any(x => x.Value.Item1.ContainsKey(Context.ConnectionId)))
                {
                    var ctx = _games.First(x => x.Value.Item1.ContainsKey(Context.ConnectionId));

                    ctx.Value.Item1.Remove(Context.ConnectionId);
                }

                return base.OnDisconnectedAsync(exception);
            }
        }

        public class ChatMessage
        {
            public string User { get; set; }

            public string Message { get; set; }
        }

        public interface IChatClient
        {
            Task ReceiveMessage(ChatMessage message);
            Task Started(InitData message);
            Task Moved(Changes message);
        }
    }
}
