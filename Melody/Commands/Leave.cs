using Melody.Services;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord;

namespace Melody.Commands
{
    public class Leave : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Leave(MusicService musicService)
        {
            _musicService = musicService;
        }

        [Command("leave")]
        [Alias("l")]
        public async Task LeaveAsync()
        {
            var user = Context.User as SocketGuildUser;
            var textChannel = Context.Channel as ITextChannel;
            await textChannel.TriggerTypingAsync();
            await ReplyAsync(null, false, await _musicService.LeaveAsync(user.VoiceChannel, Context.Guild.Id));
        }
    }
}
