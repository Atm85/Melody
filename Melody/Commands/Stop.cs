using Melody.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Victoria;

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
            await ReplyAsync(null, false, await _musicService.StopAsync(Context.Guild.Id));
        }
    }
}
