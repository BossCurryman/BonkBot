using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using Discord.Commands;

namespace BonkBot
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        //TODO: support multiple users at once
        // Will bonk a user from the server they are in
        [Command ("bonk",RunMode = RunMode.Async)]
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
                await user.ModifyAsync(x => x.Mute = true);
                UpdateBonkScores(sender, user);
                ConnectAndPlayBonk(user.VoiceChannel, user);

                
                string s = "Bonk, go to horny jail " + user.Username + "\n";
                await ReplyAsync(s);
            }
            else
            {
                await ReplyAsync("You arent high enough rank to bonk " + user.ToString());
            }
                      
        }

        [Command("uwu", RunMode = RunMode.Async)]
        [Summary("Quarantines a furry")]
        public async Task UwuCommandAsync([Remainder][Summary("The user(s) to bonk, cannot be multiple users")] SocketGuildUser user)
        {
            /*
            bool auth;
            SocketGuildUser sender = Context.User as SocketGuildUser;
            try
            {
                auth = Global.authManager.AuthoriseActionOnUser(sender, user);
            }
            catch (EqualUserAuthroityException)
            {
                auth = true;
            }

            if (auth)
            {
                await user.ModifyAsync(x => x.Mute = true);
                UpdateBonkScores(sender, user);
                ConnectAndPlayBonk(user.VoiceChannel, user);


                string s = "Bonk, go to horny jail " + user.Username + "\n";
                await ReplyAsync(s);
            }
            else
            {
                await ReplyAsync("You arent high enough rank to bonk " + user.ToString());
            }
            */

        }

        private async Task ConnectAndPlayBonk(SocketVoiceChannel chan, SocketGuildUser user)
        {
            if(chan is null)
            {

            }
            else
            {
                IAudioClient client = await chan.ConnectAsync(true);
                await SendAsync(client, $"Bonk.mp3");
                await chan.DisconnectAsync();
            }
            await user.ModifyAsync(x => x.Channel = null); 
            await user.ModifyAsync(x => x.Mute = false);

        }

        private Process CreateAudioStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }

        private async Task SendAsync(IAudioClient client, string path)
        {
            // Create FFmpeg using the previous example
            using (var ffmpeg = CreateAudioStream(path))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); }
            }
        }

        //updates the score for the bonkee and the bonker
        private void UpdateBonkScores(SocketGuildUser sender, SocketGuildUser bonkee)
        {
            //guild of sender and bonkee should never be different, no bonking across guilds
            SocketGuild g = sender.Guild;
            Dictionary<SocketGuildUser, UserValues> dict = Global.authManager.AuthDictionary[g];
            if(!dict.ContainsKey(sender))
            {
                Global.authManager.AuthDictionary.Add(g, sender, new UserValues(Authority.BonkGuard));
            }
            if (!dict.ContainsKey(bonkee))
            {
                Global.authManager.AuthDictionary.Add(g, bonkee, new UserValues(Authority.BonkGuard));
            }
            //cant use dict here becasue modifying dict will not modify the resource it came from
            Global.authManager.AuthDictionary[g, sender].TimesBonking++;
            Global.authManager.AuthDictionary[g, bonkee].TimesBonked++;
        }

        //TODO: non violent escort


        //make this happen upon application shutdown and intermittently, or maybe every change without awaiting it
        [Command("debugSave")]
        [Summary("debug shits")]
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
