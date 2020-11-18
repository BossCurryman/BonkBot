using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace BonKBot
{
    public class CommunicationModule : ModuleBase<SocketCommandContext>
    {
        //displays the help for commands
        [Command("help")]
        [Summary("Gives help")]
        public async Task HelpCommandAsync()
        {
            List<CommandInfo> commands = new List<CommandInfo>(Global.service.Commands);
            EmbedBuilder builder = new EmbedBuilder();
            foreach (CommandInfo command in commands)
            {
                string aliases = "";
                foreach (string s in command.Aliases)
                {
                    aliases += (s + ", ");
                }
                string embedFieldText = "Aliases: " + aliases + "\n";
                embedFieldText += command.Summary ?? "no description available\n";
                builder.AddField(command.Name, embedFieldText);
            }
            await ReplyAsync("Here are my commands: ", false, builder.Build());
        }

        //displays the user whomst invoked's authority        
        [Command("authority")]
        [Summary("Shows you what rank you are in the horny prison industrial complex")]
        [Alias("auth", "rank")]
        public async Task AuthCommandAsync()
        {
            Authority a;
            SocketGuild g = Context.Guild;
            SocketGuildUser u = Context.User as SocketGuildUser;
            if (Global.authManager.AuthDictionary[g].ContainsKey(u))
            {
                a = Global.authManager.AuthDictionary[g][u].Auth;
            }
            else
            {
                a = Authority.BonkGuard;
            }
            string s = "You are a " + a.ToString();
            await ReplyAsync(s);
        }

        [Command("authority")]
        [Summary("Shows you the rank of another in the horny prison industrial complex")]
        [Alias("auth", "rank")]
        public async Task AuthCommandAsync([Remainder][Summary("The user who's rank is being checked")] SocketGuildUser user)
        {
            Authority a;
            SocketGuild g = Context.Guild;
            SocketGuildUser u = user;
            if (Global.authManager.AuthDictionary[g].ContainsKey(u))
            {
                a = Global.authManager.AuthDictionary[g][u].Auth;
            }
            else
            {
                a = Authority.BonkGuard;
            }
            string s = u.ToString() + " is a " + a.ToString();
            await ReplyAsync(s);
        }

        [Command("Time")]
        [Summary("Shows the servers time and the time local to the user whom invoked")]
        [Alias("tm")]
        public async Task TimeCommandAsync()
        {
            DateTimeOffset serverTime = Context.Message.CreatedAt;
            await ReplyAsync(serverTime.ToString());
        }

        //TODO: add command which shows the authorities of of most of the guild

        [Command("stats")]
        [Summary("Shows the servers time and the time local to the user whom invoked")]
        [Alias("st", "stat")]
        public async Task StatsCommandAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();
            UserValues v = Global.authManager.AuthDictionary[Context.Guild][Context.User as SocketGuildUser];
            uint bonkee = v.TimesBonked;
            uint bonker = v.TimesBonking;
            builder.AddField("Times bonked", bonkee);
            builder.AddField("Times bonking someone", bonker);
            builder.Color = new Color(255, 0, 0);
            await ReplyAsync("Your stats, my liege:", false, builder.Build());
        }
    }
}
