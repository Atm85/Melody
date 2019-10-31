using Melody.Services;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class Stop : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Stop(MusicService musicService)
        {
            _musicService = musicService;
        }


        [Command("stop")]
        [Alias("s")]
        public async Task StopAsync()
        {
            var textChannel = Context.Channel as ITextChannel;
            await textChannel.TriggerTypingAsync();
            await ReplyAsync(null, false, await _musicService.StopAsync(Context.Guild.Id));
        }
    }
}
