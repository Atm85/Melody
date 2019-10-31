using Melody.Services;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class Pause : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Pause(MusicService musicService)
        {
            _musicService = musicService;
        }


        [Command("pause")]
        [Alias("pp")]
        public async Task PauseAsync()
        {
            var textChannel = Context.Channel as ITextChannel;
            await textChannel.TriggerTypingAsync();
            await ReplyAsync(null, false, await _musicService.PauseAsync(Context.Guild.Id));
        }
    }
}
