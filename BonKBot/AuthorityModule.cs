using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace BonKBot
{
    public class AuthorityModule : ModuleBase<SocketCommandContext>
    {
        //TODO assign a user an authority

        [Command("promote")]
        [Summary("Promotes a user one rank, this can only be performed upon a user by a user of higher ranking")]
        [Alias("prom", "pr")]
        public Task PromoteCommandAsync([Remainder][Summary("Promotes a user 1 rank, only higher ranking user can do this")] SocketGuildUser user)
        {
            try
            {
                Authority a = Global.authManager.PromoteUser((SocketGuildUser)Context.User, user);
                ReplyAsync(user.ToString() + " is now " + a.ToString());
            }
            catch(EqualUserAuthroityException)
            {
                ReplyAsync("You cant promote someone of equal power silly.");
            }
            catch (UnauthorisedAuthroityException)
            {
                ReplyAsync("You are not authorised to promote this person.");
            }           
            return Task.CompletedTask;
        }

        [Command("demote")]
        [Summary("Demotes a user one rank, this can only be performed upon a user by a user of higher ranking")]
        [Alias("dem")]
        public Task DemoteCommandAsync([Remainder][Summary("Demotes a user 1 rank, only higher ranking user can do this")] SocketGuildUser user)
        {
            try
            {
                Authority a = Global.authManager.DemoteUser((SocketGuildUser)Context.User, user);
                ReplyAsync(user.ToString() + " is now " + a.ToString());
            }
            catch (EqualUserAuthroityException)
            {
                ReplyAsync("You cant demote someone of equal power silly.");
            }
            catch (UnauthorisedAuthroityException)
            {
                ReplyAsync("You are not authorised to demote this person.");
            }            
            return Task.CompletedTask;
        }

    }
}
