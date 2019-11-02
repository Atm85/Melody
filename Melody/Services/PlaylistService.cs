using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var dictionary = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<string, List<List<string>>>>>(json);
            var list = new List<List<string>>();
            
            if (!dictionary.ContainsKey(userId))
            {
                dictionary.Add(userId, new Dictionary<string, List<List<string>>>());
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
            var dictionary = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<string, List<List<string>>>>>(json);
            var list = new List<string>();

            if (!dictionary.ContainsKey(userId))
            {
                embed.WithDescription("You do not author any playlists! `.list create` to create one");
                return embed.Build();
            }

            if (!dictionary[userId].ContainsKey(name))
            {
                embed.WithDescription($"Playlist [{name}] does not exist!");
                return embed.Build();
            }
            
            embed.WithDescription($"Removed list [{name}] and [{dictionary[userId][name].Count}] tracks");
            dictionary[userId].Remove(name);
            
            string jsonString = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            File.WriteAllText(path + "/" + file, jsonString);
            
            return embed.Build();
        }

        internal static async Task<Embed> AddSongAsync(ulong userId, string playList, string query)
        {
            var results = await MusicService.lavaRestClient.SearchYouTubeAsync(query);
            var track = results.Tracks.FirstOrDefault();
            var name = track.Title;

            var embed = new EmbedBuilder();

            string json = File.ReadAllText(path + "/" + file);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<string, List<List<string>>>>>(json);
            var list = new List<string>();

            if (!dictionary.ContainsKey(userId))
            {
                dictionary.Add(userId, new Dictionary<string, List<List<string>>>());
            }

            if (!dictionary[userId].ContainsKey(playList))
            {
                embed.WithDescription("unknown playlist! try creating one?");
                return embed.Build();
            }

            list.Add($"{dictionary[userId][playList].Count + 1}");
            list.Add(name);
            list.Add($"{track.Uri}");
            dictionary[userId][playList].Add(list);

            embed.WithDescription($"Added song to playlist [{playList}]:\n[{name}]({track.Uri})");
            string jsonString = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            File.WriteAllText(path + "/" + file, jsonString);

            return embed.Build();
        }

        internal static List<string> GetPlaylist(ulong userId, string playlist)
        {
            string json = File.ReadAllText(path + "/" + file);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<string, List<List<string>>>>>(json);
            var list = new List<string>();
            
            foreach (var i in dictionary[userId][playlist])
            {
                list.Add(i[2]);
            }
            
            return list;
        }
    }
}
