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
    public class Skip : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Skip(MusicService musicService)
        {
            _musicService = musicService;
        }


        [Command("skip")]
        [Alias("ss")]
        public async Task SkipAsync()
        {
            await ReplyAsync(null, false, await _musicService.SkipAsync(Context.Guild.Id));
        }
    }
}
