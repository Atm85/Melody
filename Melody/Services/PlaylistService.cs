using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Melody.Services
{
    class PlaylistService
    {
        private const string path = "Resources";
        private const string file = "playlistData.json";
        
        internal static Embed CreatePlaylist(ulong userId, string name)
        {
            var embed = new EmbedBuilder();

            string json = File.ReadAllText(path + "/" + file);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<string, List<string>>>>(json);
            var list = new List<string>();

            if (!dictionary.ContainsKey(userId))
            {
                dictionary.Add(userId, new Dictionary<string, List<string>>());
            }

            if (!dictionary[userId].ContainsKey(name))
            {
                dictionary[userId].Add(name, list);
            }

            embed.WithTitle($"Created playlist: [{name}]");
            embed.AddField("Begin adding songs with command:", ".list add <playlist name> <song name>", false);

            string jsonString = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            File.WriteAllText(path + "/" + file, jsonString);

            return embed.Build();
        }

        internal static Embed DeletePlaylist(ulong userId, string name)
        {
            var embed = new EmbedBuilder();

            string json = File.ReadAllText(path + "/" + file);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<string, List<string>>>>(json);
            var list = new List<string>();

            if (!dictionary.ContainsKey(userId))
            {
                embed.WithDescription("You do not author any playlists! `.list create` to create one");
                return embed.Build();
            }

            if (dictionary[userId].ContainsKey(name))
            {
                embed.WithDescription($"Removed list [{name}] and [{dictionary[userId][name].Count}] tracks");
                dictionary[userId].Remove(name);
            }
            else
            {
                embed.WithDescription($"Playlist [{name}] does not exist!");
            }
            string jsonString = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            File.WriteAllText(path + "/" + file, jsonString);
            
            return embed.Build();
        }

        internal static Embed AddSong(ulong userId, string playList, string name)
        {
            var embed = new EmbedBuilder();

            string json = File.ReadAllText(path + "/" + file);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<string, List<string>>>>(json);

            if (!dictionary.ContainsKey(userId))
            {
                dictionary.Add(userId, new Dictionary<string, List<string>>());
            }

            if (!dictionary[userId].ContainsKey(playList))
            {
                embed.WithDescription("unknown playlist! try creating one?");
                return embed.Build();
            }

            dictionary[userId][playList].Add(name);
            embed.WithDescription($"Added song to playlist [{playList}]:\n[{name}]");

            string jsonString = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            File.WriteAllText(path + "/" + file, jsonString);

            return embed.Build();
        }

        internal static List<string> GetPlaylist(ulong userId, string playlist)
        {
            string json = File.ReadAllText(path + "/" + file);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<string, List<string>>>>(json);
            return dictionary[userId][playlist];
        }
    }
}
