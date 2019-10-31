using Melody.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using System.Linq;

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
        public async Task PlayAsync([Remainder] string query)
        {
            string[] args = Context.Message.Content.Split(" ");
            ulong userId = Context.Message.Author.Id;

            var user = Context.User as SocketGuildUser;
            var textChannel = Context.Channel as ITextChannel;
            if (user.VoiceChannel == null)
            {
                await ReplyAsync(null, false, await _musicService.ConnectAsync(user.VoiceChannel, textChannel));
            }
            else
            {
                await ReplyAsync(null, false, await _musicService.ConnectAsync(user.VoiceChannel, textChannel));
                await ReplyAsync(null, false, await _musicService.PlayAsync(query, Context.Guild.Id));
            }
        }

    }
}
