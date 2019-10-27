using Melody.Services;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class Leave : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Leave(MusicService musicService)
        {
            _musicService = musicService;
        }

        [Command("leave")]
        [Alias("l")]
        public async Task LeaveAsync()
        {
            var user = Context.User as SocketGuildUser;
            await ReplyAsync(null, false, await _musicService.LeaveAsync(user.VoiceChannel, Context.Guild.Id));
        }
    }
}
