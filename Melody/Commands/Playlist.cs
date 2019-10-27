using Discord.Commands;
using Melody.Services;
using System;
using System.Collections.Generic;
using System.Text;
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
            await ReplyAsync(null, false, await _musicService.PlaylistAsync(PlaylistService.GetPlaylist(userId, query), guildId));
        }
    }
}
