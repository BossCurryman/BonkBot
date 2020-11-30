using BonkInterfacing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;



namespace BonkBot
{
    public enum Authority
    {
        Warden,
        HornyCellSentinel,
        BonkGuard,
        rank4,
        rank5,
    }

    public class AuthorityManager
    {
        
        internal CompoundKeyDictionary<SocketGuild, SocketGuildUser, UserValues> AuthDictionary { get; private set; }
            = new CompoundKeyDictionary<SocketGuild, SocketGuildUser, UserValues>();
        private List<SocketGuild> guilds;

        //string dictPath = @"AuthorityDictionary.JSON";

        public AuthorityManager(IReadOnlyCollection<SocketGuild> botGuilds)
        {
            guilds = new List<SocketGuild>(botGuilds);
        }

        public AuthorityManager()
        {
        }

        public Task AddGuildsFromCollection(IReadOnlyCollection<SocketGuild> guilds )
        {
            this.guilds = new List<SocketGuild>(guilds);
            return Task.CompletedTask;
        }

        public void AddOwners()
        {
            foreach(SocketGuild guild in guilds)
            {
                SocketUser o;
                if (guild.Owner != null)
                {
                    o = guild.Owner;
                }
                else
                {
                    o = Global.client.GetUser(guild.OwnerId);
                }

                if (AuthDictionary[guild].ContainsKey(o as SocketGuildUser))
                {
                    //pass
                }
                else
                {
                    AuthDictionary[guild].Add(o as SocketGuildUser, new UserValues(Authority.Warden));
                }         
            }
        }

        //whenevere the bot enters a new server, the owner is auto granted warden priveleges
        public Task AddedToServer(SocketGuild s)
        {
            try
            {
                AuthDictionary.Add(s, s.Owner, new UserValues(Authority.Warden));
                //AuthDictionary.Add(s, Global.Boss_Curryman, new UserValues(Authority.Warden));
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Owner already exists");
            }            
            return Task.CompletedTask;
        }

        public SocketGuildUser CompareAuthorities(SocketGuildUser user1, SocketGuildUser user2)
        {
            Authority auth1;
            Authority auth2;
            SocketGuild g = user1.Guild;
            if (AuthDictionary[g].ContainsKey(user1))
            {
                auth1 = AuthDictionary[g][user1].Auth;
            }
            else
            {
                auth1 = Authority.BonkGuard;
            }

            if (AuthDictionary[g].ContainsKey(user2))
            {
                auth2 = AuthDictionary[g][user2].Auth;
            }
            else
            {
                auth2 = Authority.BonkGuard;
            }

            int comp = auth1.CompareTo(auth2);

            return comp switch
            {
                1 => user1,
                0 => throw new EqualUserAuthroityException("The users have the same authority"),
                -1 => user2,
                _ => throw new CalculationAuthorityException("Comparison case block broke out unexpectedly")
            };
        }

        public bool AuthoriseActionOnUser(SocketGuildUser user1, SocketGuildUser user2)
        {
            Authority auth1;
            Authority auth2;
            SocketGuild g = user1.Guild;
            if (AuthDictionary[g].ContainsKey(user1))
            {
                auth1 = AuthDictionary[g][user1].Auth;
            }
            else
            {
                auth1 = Authority.BonkGuard;
            }

            if (AuthDictionary[g].ContainsKey(user2))
            {
                auth2 = AuthDictionary[g][user2].Auth;
            }
            else
            {
                auth2 = Authority.BonkGuard;
            }

            int comp = auth1.CompareTo(auth2);
            return comp switch
            {
                1 => false,
                0 => throw new EqualUserAuthroityException("The users have the same authority"),
                -1 => true,
                _ => throw new CalculationAuthorityException("Comparison case block broke out unexpectedly")
            };
        }

        public bool AuthoriseAction(SocketGuildUser user1, Authority auth2)
        {
            Authority auth1;
            SocketGuild g = user1.Guild;
            if (AuthDictionary[g].ContainsKey(user1))
            {
                auth1 = AuthDictionary[g][user1].Auth;
            }
            else
            {
                auth1 = Authority.BonkGuard;
            }
            int comp = auth1.CompareTo(auth2);

            return comp switch
            {
                1 => true,
                0 => true,
                -1 => false,
                _ => throw new CalculationAuthorityException("Comparison case block broke out unexpectedly")
            };
        }

        public async Task<Authority> PromoteUser(SocketGuildUser executor, SocketGuildUser user)
        {

            bool authorised = AuthoriseActionOnUser(executor, user);
            SocketGuild g = executor.Guild;
            if (authorised)
            {
                if (AuthDictionary[g].ContainsKey(user))
                {
                    if (AuthDictionary[g][user].Auth == Authority.Warden)
                    {
                        throw new UnauthorisedAuthroityException();
                    }
                    else
                    {
                        AuthDictionary[g][user].Auth -= 1;
                    }
                }
                else
                {
                    AuthDictionary.Add(user.Guild, user, new UserValues(Authority.HornyCellSentinel));
                }
            }
            else
            {
                throw new UnauthorisedAuthroityException();
            }

            await WriteUsersToFileAsync();

            return AuthDictionary[g][user].Auth;
        }

        public async Task<Authority> DemoteUser(SocketGuildUser executor, SocketGuildUser user)
        {

            bool authorised = AuthoriseActionOnUser(executor, user);
            SocketGuild g = executor.Guild;


            if (authorised)
            {
                if (AuthDictionary[g].ContainsKey(user))
                {
                    if (AuthDictionary[g][user].Auth == Authority.Warden)
                    {
                        throw new UnauthorisedAuthroityException();
                    }
                    else
                    {
                        AuthDictionary[g][user].Auth += 1;
                    }
                }
                else
                {
                    AuthDictionary.Add(user.Guild, user, new UserValues(Authority.HornyCellSentinel));
                }
            }
            else
            {
                throw new UnauthorisedAuthroityException();
            }

            await WriteUsersToFileAsync();

            return AuthDictionary[g][user].Auth;
        }

        private async Task WriteUsersToFileAsync()
        {
            await AuthDictionary.ExportGuildDictionaryAsync(@"exportTest.json");
        }

        //TODO: read und write a file containing all Authority shits
        private void ReadAuthorities()
        {

        }

    }
}
