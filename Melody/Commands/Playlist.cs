using Discord;
using Discord.Commands;
using Melody.Services;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class Playlist : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Playlist(MusicService musicService)
        {
            _musicService = musicService;
        }

        [Command("playlist")]
        [Alias("pl")]
        public async Task PlaylistAsync([Remainder]string query)
        {
            ulong userId = Context.Message.Author.Id;
            ulong guildId = Context.Guild.Id;
            var textChannel = Context.Channel as ITextChannel;
            await textChannel.TriggerTypingAsync();
            await _musicService.PlaylistAsync(PlaylistService.GetPlaylist(userId, query), guildId, textChannel);
        }
    }
}
