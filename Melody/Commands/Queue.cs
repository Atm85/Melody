using Melody.Services;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class Queue : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Queue(MusicService musicService)
        {
            _musicService = musicService;
        }


        [Command("queue")]
        [Alias("q")]
        public async Task QueueAsync()
        {
            var textChannel = Context.Channel as ITextChannel;
            await textChannel.TriggerTypingAsync();
            await ReplyAsync(null, false, await _musicService.QueueAsync(Context.Guild.Id));
        }
    }
}
