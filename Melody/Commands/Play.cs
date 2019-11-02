using Melody.Services;
using Discord.Commands;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;

namespace Melody.Commands
{
    public class Play : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Play(MusicService musicService)
        {
            _musicService = musicService;
        }

        [Command("play")]
        [Alias("p")]
        public async Task PlayAsync([Remainder] string query)
        {
            string[] args = Context.Message.Content.Split(" ");
            ulong userId = Context.Message.Author.Id;
            ulong guildId = Context.Guild.Id;
            var embed = new EmbedBuilder();

            var user = Context.User as SocketGuildUser;
            var textChannel = Context.Channel as ITextChannel;
            if (user.VoiceChannel == null)
            {
                await textChannel.TriggerTypingAsync();
                embed.WithDescription("You are not connected to a voice channel!");
                await ReplyAsync(null, false, embed.Build());
            }
            else
            {
                await textChannel.TriggerTypingAsync();
                await _musicService.ConnectAsync(user.VoiceChannel, textChannel);

                await textChannel.TriggerTypingAsync();
                await ReplyAsync(null, false, await _musicService.PlayAsync(query, Context.Guild.Id));
            }
        }

    }
}
