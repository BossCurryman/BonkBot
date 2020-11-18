using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.API;
using Discord.Commands;

namespace BonKBot
{
    class CommandList
    {
        public char InvokeChar { get; } = '>';

        private readonly CommandService commands;
        private readonly DiscordSocketClient client;

        public CommandList(DiscordSocketClient client, CommandService service)
        {
            this.client = client;
            commands = service;
        }

        public async Task InstallCommandsAsync()
        {
            client.MessageReceived += RecievedCommandAsync;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

        }

        public async Task RecievedCommandAsync(SocketMessage msg)
        {
            SocketUserMessage message = msg as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (message.Author.IsBot || !message.HasCharPrefix(InvokeChar, ref argPos)) return;

            SocketCommandContext context = new SocketCommandContext(client, message);
            await commands.ExecuteAsync(context: context, argPos: argPos, services: null);
        }       
    }
}
