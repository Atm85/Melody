using Melody.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class Join : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Join(MusicService musicService)
        {
            _musicService = musicService;
        }

        [Command("join")]
        [Alias("j")]
        public async Task JoinAsync()
        {
            var embed = new EmbedBuilder();

            var user = Context.User as SocketGuildUser;
            var textChannel = Context.Channel as ITextChannel;
            await textChannel.TriggerTypingAsync();

            if (user.VoiceChannel == null)
            {
                embed.WithDescription("You are not connected to a voice channel!");
                await ReplyAsync(null, false, embed.Build());
            }
            else
            {
                embed.WithDescription($"Now connected to: {user.VoiceChannel.Name}");
                await ReplyAsync(null, false, embed.Build());
                await _musicService.ConnectAsync(user.VoiceChannel, textChannel);
            }
        }
    }
}
