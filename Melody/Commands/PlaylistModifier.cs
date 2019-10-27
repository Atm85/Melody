using Discord;
using Discord.Commands;
using Melody.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class PlaylistModifier : ModuleBase<SocketCommandContext>
    {
        [Command("list")]
        public async Task ModifierAsync([Remainder]string parameters)
        {
            await Context.Channel.TriggerTypingAsync();

            string[] args = Context.Message.Content.Split(" ");
            ulong userId = Context.Message.Author.Id;
            var embed = new EmbedBuilder();

            switch (args[1])
            {
                case "create":
                    if (args.Length == 3)
                    {
                        embed.WithDescription($"Created playlist: {args[2]}");
                        embed.AddField("Begin adding songs with command:", ".list add <playlist name> <song name>", false);
                        await ReplyAsync(null, false, PlaylistService.CreatePlaylist(userId, args[2]));
                        return;
                    }
                    embed.WithDescription(".list create <playlist name>");
                    await ReplyAsync(null, false, embed.Build());
                    return;
                case "add":
                    if (args.Length > 3)
                    {
                        var array = args.Where((item, index) => index >= 3).ToArray();
                        var name = string.Join(" ", array);
                        await ReplyAsync(null, false, PlaylistService.AddSong(userId, args[2], name));
                        return;
                    }
                    embed.WithDescription(".list add <playlist name> <song name>");
                    await ReplyAsync(null, false, embed.Build());
                    return;
                default:
                    await ReplyAsync("unknown sub-command");
                    return;
            }
        }
    }
}
