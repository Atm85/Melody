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
            var user = Context.User as SocketGuildUser;
            var textChannel = Context.Channel as ITextChannel;
            await textChannel.TriggerTypingAsync();
            await ReplyAsync(null, false, await _musicService.ConnectAsync(user.VoiceChannel, textChannel));
        }
    }
}
