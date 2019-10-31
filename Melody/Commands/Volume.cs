using Melody.Services;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class Volume : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Volume(MusicService musicService)
        {
            _musicService = musicService;
        }


        [Command("volume")]
        [Alias("v")]
        public async Task VolumeAsync(int vol)
        {
            var textChannel = Context.Channel as ITextChannel;
            await textChannel.TriggerTypingAsync();
            await ReplyAsync(null, false, await _musicService.SetVolumeAsync(vol, Context.Guild.Id));
        }
    }
}
