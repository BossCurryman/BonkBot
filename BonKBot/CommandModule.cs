using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace BonKBot
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        //TODO: support multiple users at once
        // Will bonk a user from the server they are in
        [Command ("bonk")]
        [Summary ("Sends a user to horny jail.")]
        public async Task BonkCommandAsync([Remainder][Summary ("The user(s) to bonk, cannot be multiple users")] SocketGuildUser user)
        {
            bool auth;
            SocketGuildUser sender = Context.User as SocketGuildUser;
            try
            {
                auth = Global.authManager.AuthoriseActionOnUser(sender, user);
            }
            catch(EqualUserAuthroityException)
            {
                auth = true;
            }

            if(auth)
            {
                await UpdateBonkScores(sender, user);
                await user.ModifyAsync(x => x.Channel = null);
                string s = "Bonk, go to horny jail " + user.Username + "\n";
                await Context.Channel.SendMessageAsync(s);
            }
            else
            {
                await ReplyAsync("You arent high enough rank to bonk " + user.ToString());
            }
                      
        }

        private Task UpdateBonkScores(SocketGuildUser sender, SocketGuildUser bonkee)
        {
            //guild of sender and bonkee should never be different, no bonking across guilds
            SocketGuild g = sender.Guild;
            Dictionary<SocketGuildUser, UserValues> dict = Global.authManager.AuthDictionary[g];
            if(!dict.ContainsKey(sender))
            {
                dict.Add(sender, new UserValues(Authority.BonkGuard));
            }
            if (!dict.ContainsKey(bonkee))
            {
                dict.Add(bonkee, new UserValues(Authority.BonkGuard));
            }
            dict[sender].TimesBonking++;
            dict[bonkee].TimesBonked++;
            return Task.CompletedTask;
        }

        //TODO: non violent escort

    }
}
