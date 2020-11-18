using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace BonKBot
{
    public static class Global
    {
        public static CommandService service;
        public static AuthorityManager authManager;
        public static DiscordSocketClient client;
    }
}
