using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;


namespace BonKBot
{
    class Program
    {
        public static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private AuthorityManager authManager;
        private protected string token = "Nzc2Mzg0MTgzNzU1ODY2MTEy.X60GHQ.VWeK9F1lOLNu0FCe6PHZWHCuqqQ";
        public async Task MainAsync()
        {
            DiscordSocketConfig conf = new DiscordSocketConfig();
            authManager = new AuthorityManager();
            conf.AlwaysDownloadUsers = true;
            client = new DiscordSocketClient(conf);
            client.Log += Log;
            Global.client = client;
            //manage private keys better!!!!!!
            //
            
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            client.Ready += GuildsReady;
            client.GuildAvailable += GuildAvailable;
            client.GuildMembersDownloaded += GuildsDownloadedAsync;
            await Task.Delay(-1);
        }

        private async Task GuildsReady()
        {

            client.JoinedGuild += authManager.AddedToServer;
            CommandService service = new CommandService();
            CommandList comList = new CommandList(client, service);

            await comList.InstallCommandsAsync();
            
            Global.authManager = authManager;
            Global.service = service;
        }

        private Task GuildAvailable(SocketGuild g)
        {
            authManager.AuthDictionary.Add(g, new Dictionary<SocketGuildUser, UserValues>());
            return Task.CompletedTask;
        }

        private Task GuildsDownloadedAsync(SocketGuild g)
        {
            Console.WriteLine(g.Name + "Is available");
            foreach(SocketUser u in g.Users)
            {
                Console.WriteLine(u.ToString());
            }
            authManager.AddedToServer(g);
            return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
