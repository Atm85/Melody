using Melody.Services;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class Resume : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Resume(MusicService musicService)
        {
            _musicService = musicService;
        }


        [Command("resume")]
        [Alias("r")]
        public async Task ResumeAsync()
        {
            var textChannel = Context.Channel as ITextChannel;
            await textChannel.TriggerTypingAsync();
            await ReplyAsync(null, false, await _musicService.ResumeAsync(Context.Guild.Id));
        }
    }
}
