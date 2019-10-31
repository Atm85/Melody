using Melody.Services;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class Skip : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Skip(MusicService musicService)
        {
            _musicService = musicService;
        }


        [Command("skip")]
        [Alias("ss")]
        public async Task SkipAsync()
        {
            var textChannel = Context.Channel as ITextChannel;
            await textChannel.TriggerTypingAsync();
            await ReplyAsync(null, false, await _musicService.SkipAsync(Context.Guild.Id));
        }
    }
}
