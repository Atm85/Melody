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
            await ReplyAsync(null, false, await _musicService.SetVolumeAsync(vol, Context.Guild.Id));
        }
    }
}
