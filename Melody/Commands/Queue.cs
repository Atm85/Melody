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
    public class Queue : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Queue(MusicService musicService)
        {
            _musicService = musicService;
        }


        [Command("queue")]
        [Alias("q")]
        public async Task QueueAsync()
        {
            await ReplyAsync(null, false, await _musicService.QueueAsync(Context.Guild.Id));
        }
    }
}
