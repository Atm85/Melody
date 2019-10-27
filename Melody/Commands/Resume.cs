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
            await ReplyAsync(null, false, await _musicService.ResumeAsync(Context.Guild.Id));
        }
    }
}
