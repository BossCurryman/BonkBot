using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Discord.WebSocket;
using BonkInterfacing;

namespace BonkBot
{
    internal static class InterfaceExtensions
    {
        public static async Task ImportGuildDictionaryAsync
            (this CompoundKeyDictionary<SocketGuild,SocketGuildUser, UserValues> dict, string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            string s = await reader.ReadToEndAsync();
            reader.Close();

            CompoundKeyDictionary<ulong, ulong, UserValues> unElaboratedDict =
                JsonSerializer.Deserialize(s, typeof(CompoundKeyDictionary<ulong, ulong, UserValues>))
                as CompoundKeyDictionary<ulong,ulong,UserValues>;

            dict.Clear();
            foreach(CompoundKeyValueSet<ulong,ulong,UserValues> valueSet in unElaboratedDict)
            {
                SocketGuild guild = Global.client.GetGuild(valueSet.PrimaryKey);
                SocketGuildUser user = Global.client.GetUser(valueSet.SecondaryKey) as SocketGuildUser;
                dict.Add(guild, user, valueSet.Value);            
            }
        }

        public static async Task ImportUsersIntoDictionaryAsync
            (this CompoundKeyDictionary<SocketGuild, SocketGuildUser, UserValues> dict, string filePath, SocketGuild guild)
        {
            StreamReader reader = new StreamReader(filePath);
            string s = await reader.ReadToEndAsync();
            reader.Close();

            CompoundKeyDictionary<ulong, ulong, UserValues> unElaboratedDict =
                JsonSerializer.Deserialize(s, typeof(CompoundKeyDictionary<ulong, ulong, UserValues>))
                as CompoundKeyDictionary<ulong, ulong, UserValues>;

            Dictionary<ulong, UserValues> userDict = unElaboratedDict[guild.Id];
            foreach(KeyValuePair<ulong,UserValues> keySet in userDict)
            {
                SocketGuildUser user = guild.GetUser(keySet.Key);
                dict.Add(guild, user, keySet.Value);
            }
        }

        internal static async Task ExportGuildDictionaryAsync
            (this CompoundKeyDictionary<SocketGuild, SocketGuildUser, UserValues> dict, string filePath)
        {
            CompoundKeyDictionary<ulong, ulong, UserValues> writeDictionary = new CompoundKeyDictionary<ulong, ulong, UserValues>();

            foreach(CompoundKeyValueSet<SocketGuild,SocketGuildUser,UserValues> valueSet in dict)
            {
                writeDictionary.Add(valueSet.PrimaryKey.Id, valueSet.SecondaryKey.Id, valueSet.Value);
            }
            string s = JsonSerializer.Serialize(writeDictionary, new JsonSerializerOptions { WriteIndented = true });
            StreamWriter writer = new StreamWriter(filePath);
            await writer.WriteAsync(s);
            writer.Close();
        }

    }
}
