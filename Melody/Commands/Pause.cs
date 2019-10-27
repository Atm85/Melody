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
    public class Pause : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Pause(MusicService musicService)
        {
            _musicService = musicService;
        }


        [Command("pause")]
        [Alias("pp")]
        public async Task PauseAsync()
        {
            await ReplyAsync(null, false, await _musicService.PauseAsync(Context.Guild.Id));
        }
    }
}
