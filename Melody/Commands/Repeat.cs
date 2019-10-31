using Discord;
using Discord.Commands;
using Melody.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class Repeat : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        private const string path = "Resources";
        private const string file = "config.json";

        public Repeat(MusicService musicService)
        {
            _musicService = musicService;
        }

        [Command("repeat")]
        [Alias("rp")]
        public async Task RepeatAsync()
        {
            var textChannel = Context.Channel as ITextChannel;
            await textChannel.TriggerTypingAsync();

            var embed = new EmbedBuilder();
            string jsonString = File.ReadAllText(path + "/" + file);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<string, bool>>>(jsonString);
            if (dictionary.ContainsKey(Context.Guild.Id))
            {
                if (dictionary[Context.Guild.Id]["repeat"])
                {
                    embed.WithDescription("Repeat disabled!");
                    await ReplyAsync(null, false, embed.Build());
                    dictionary[Context.Guild.Id]["repeat"] = false;
                } else
                {
                    embed.WithDescription("Repeat enabled!");
                    await ReplyAsync(null, false, embed.Build());
                    dictionary[Context.Guild.Id]["repeat"] = true;
                }

                string jsonData = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
                File.WriteAllText(path + "/" + file, jsonData);
            }
        }
    }
}
