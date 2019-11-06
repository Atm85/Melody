using Melody.Services;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

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
            var textChannel = Context.Channel as ITextChannel;
            var user = Context.User as IGuildUser;
            await textChannel.TriggerTypingAsync();
            if (user.GuildPermissions.Administrator)
            {
                await ReplyAsync(null, false, await _musicService.SkipAsync(Context.Guild.Id));
            } 
            else
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("Skipping!");
                embed.WithDescription("Must contain majority of all users in curent voice channel!");
                embed.WithFooter("React bellow...");
                var m = await ReplyAsync(null, false, embed.Build());
                var r = new Emoji("☑");
                await m.AddReactionAsync(r);
            }
        }
    }
}
