using Melody.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class Play : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Play(MusicService musicService)
        {
            _musicService = musicService;
        }

        [Command("play")]
        [Alias("p")]
        public async Task PlayAsync([Remainder]string query)
        {
            await ReplyAsync(null, false, await _musicService.PlayAsync(query, Context.Guild.Id));
        }

    }
}
