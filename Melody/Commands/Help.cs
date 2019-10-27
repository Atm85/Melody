using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        private string avatar;
        [Command("help")]
        [Alias("h")]
        public async Task HelpAsync()
        {
            EmbedBuilder embed = new EmbedBuilder();
            SocketUser user = Context.Message.Author;

            if (user.GetAvatarUrl() == null)
            {
                avatar = user.GetDefaultAvatarUrl();
            }
            else
            {
                avatar = user.GetAvatarUrl();
            }

            embed.WithAuthor(user.ToString(), avatar);
            embed.WithTitle(Context.Client.CurrentUser.Username.ToString() + " " + "Command options!");
            embed.WithThumbnailUrl("https://i.imgur.com/dq4HKJr.png");
            embed.WithDescription("**INFO: this bot is in `beta` and some of these features may not have full functionality yet*");
            embed.AddField("`.j` / `.join`", "Joins voice channel that you are connected to!");
            embed.AddField("`.p` / `.play`", "Starts playing in connected voice channel!");
            embed.AddField("`.pp` / `.pause`", "Pause playback!");
            embed.AddField("`.r` / `.resume`", "Resumes playback!");
            embed.AddField("`.ss` / `.skip`", "Stops current song and moves to the next! (requires majority of all users in current voice channel)");
            embed.AddField("`.v` / `.volume`", "Ajusts the players volume!");
            embed.AddField("`.s` / `.stop`", "Stops playback! (removes all songs from queue)");
            embed.AddField("`.l` / `.leave`", "Leave current voice channel!");
            embed.AddField("`.q` / `.queue`", "View all songs in queue!");
            embed.AddField("`.rp` / `.repeat`", "Infinate queue! (at the end of a song, it is re-added to queue at end)");
            embed.AddField("`.list`", "Playlist manager main command!");
            embed.AddField("`.list create <playlist name>`", "Create new playlist!");
            embed.AddField("`.list del <playlist name>`", "Delete playlist and all its contents!");
            embed.AddField("`.list add <playlist name> <track name>`", "Adds a track to the playlist");
            embed.AddField("`.list rem <playlist name> <track name>`", "removes a track from playlist");
            await ReplyAsync(null, false, embed.Build());
        }
    }
}
