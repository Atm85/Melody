using Discord;
using Discord.Commands;
using Melody.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class PlaylistModifier : ModuleBase<SocketCommandContext>
    {
        [Command("list")]
        public async Task ModifierAsync([Remainder]string parameters)
        {
            await Context.Channel.TriggerTypingAsync();

            string[] args = parameters.Split(" ");
            ulong userId = Context.Message.Author.Id;

            var textChannel = Context.Channel as ITextChannel;
            await textChannel.TriggerTypingAsync();

            var embed = new EmbedBuilder();

            switch (args[0])
            {
                case "create":
                    if (args.Length == 2)
                    {
                        embed.WithDescription($"Created playlist: {args[1]}");
                        embed.AddField("Begin adding songs with command:", ".list add <playlist name> <song name>", false);
                        await ReplyAsync(null, false, PlaylistService.CreatePlaylist(userId, args[1]));
                        return;
                    }
                    embed.WithDescription(".list create <playlist name>");
                    await ReplyAsync(null, false, embed.Build());
                    return;
                case "del":
                    await ReplyAsync(null, false, PlaylistService.DeletePlaylist(userId, args[1]));
                    return;
                case "add":
                    if (args.Length > 2)
                    {
                        var array = args.Where((item, index) => index >= 2).ToArray();
                        var name = string.Join(" ", array);
                        await ReplyAsync(null, false, await PlaylistService.AddSongAsync(userId, args[1], name));
                        return;
                    }
                    embed.WithDescription(".list add <playlist name> <song name>");
                    await ReplyAsync(null, false, embed.Build());
                    return;
                case "rem":
                    return;
                default:
                    await ReplyAsync("unknown sub-command");
                    return;
            }
        }
    }
}
