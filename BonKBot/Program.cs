﻿using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Audio;
using Discord.WebSocket;

//TODO: Comment everything
namespace BonkBot
{
    class Program
    {
        public static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private AuthorityManager authManager;
        private protected string token;

        //
        public async Task MainAsync()
        {
            token = OpenToken();

            DiscordSocketConfig conf = new DiscordSocketConfig();
            authManager = new AuthorityManager();
            conf.AlwaysDownloadUsers = true;
            client = new DiscordSocketClient(conf);
            client.Log += Log;
            Global.client = client;

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            client.Ready += GuildsReady;
            client.GuildAvailable += GuildAvailable;
            client.GuildMembersDownloaded += GuildsDownloadedAsync;

            await Task.Delay(-1);

        }

        private string OpenToken()
        {
            StreamReader reader = new StreamReader("Token.txt");
            return reader.ReadToEnd();
        }

        private async Task GuildsReady()
        {
            client.JoinedGuild += authManager.AddedToServer;
            CommandService service = new CommandService();
            CommandList comList = new CommandList(client, service);

            await comList.InstallCommandsAsync();
            
            Global.authManager = authManager;
            Global.service = service;

            //await authManager.AuthDictionary.ImportGuildDictionaryAsync(@"exportTest.json");
        }

        private Task GuildAvailable(SocketGuild g)
        {
            return Task.CompletedTask;
        }

        private async Task GuildsDownloadedAsync(SocketGuild g)
        {
            await authManager.AuthDictionary.ImportUsersIntoDictionaryAsync(@"exportTest.json", g);
            Console.WriteLine(g.Name + "Is available");
            foreach(SocketUser u in g.Users)
            {
                Console.WriteLine(u.ToString());
            }
            await authManager.AddedToServer(g);
            //return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
