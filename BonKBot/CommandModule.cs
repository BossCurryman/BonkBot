using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace BonkBot
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
                Console.WriteLine("Got to point 1");
                UpdateBonkScores(sender, user);

                Console.WriteLine("got to point 2");

                await user.ModifyAsync(x => x.Channel = null);
                string s = "Bonk, go to horny jail " + user.Username + "\n";
                await ReplyAsync(s);
            }
            else
            {
                await ReplyAsync("You arent high enough rank to bonk " + user.ToString());
            }
                      
        }




        //updates the score for the bonkee and the bonker
        private void UpdateBonkScores(SocketGuildUser sender, SocketGuildUser bonkee)
        {
            //guild of sender and bonkee should never be different, no bonking across guilds
            Console.WriteLine("Got to start of UpdateBonkScores");
            SocketGuild g = sender.Guild;
            Dictionary<SocketGuildUser, UserValues> dict = Global.authManager.AuthDictionary[g];
            if(!dict.ContainsKey(sender))
            {
                Console.WriteLine("Got to add sender");
                Global.authManager.AuthDictionary.Add(g, sender, new UserValues(Authority.BonkGuard));
                Console.WriteLine("complete add sender");
            }
            if (!dict.ContainsKey(bonkee))
            {
                Global.authManager.AuthDictionary.Add(g, bonkee, new UserValues(Authority.BonkGuard));
            }
            //cant use dict here becasue modifying dict will not modify the resource it came from
            Global.authManager.AuthDictionary[g, sender].TimesBonking++;
            Global.authManager.AuthDictionary[g, bonkee].TimesBonked++;
            Console.WriteLine("Got to end of UpdateBonkScores");
        }

        //TODO: non violent escort


        [Command("debugSave")]
        [Summary("Sends a user to horny jail.")]
        [Alias("ds")]
        public async Task DebugSaveCommandAsync()
        {
            if(Global.authManager.AuthoriseAction(Context.User as SocketGuildUser, Authority.Warden))
            {
                await WriteUsersToFileAsync(Context);
                await ReplyAsync("Files should be saved!");
            }
            else
            {
                await ReplyAsync("Not High enough rank");
            }
        }

        private async Task WriteUsersToFileAsync(SocketCommandContext context)
        {
            await Global.authManager.AuthDictionary.ExportGuildDictionaryAsync(@"exportTest.json");

        }
    }
}
