using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Entities;
using Victoria.Queue;

namespace Melody.Services
{
    public class MusicService
    {
        private DiscordSocketClient _client;
        private LavaRestClient _lavaRestClient;
        private LavaSocketClient _lavaSocketClient;
        private LavaPlayer _player;

        public static LavaRestClient lavaRestClient;

        public MusicService(DiscordSocketClient client, LavaRestClient lavaRestClient, LavaSocketClient lavaSocketClient)
        {
            _client = client;
            _lavaRestClient = lavaRestClient;
            _lavaSocketClient = lavaSocketClient;
            MusicService.lavaRestClient = lavaRestClient;
        }

        public Task InitializeAsync()
        {
            _client.Ready += ClientReadyAsync;
            _client.ReactionAdded += OnReactionAdd;
            _lavaSocketClient.Log += LogAsync;
            _lavaSocketClient.OnTrackFinished += OnTrackFinished;
            return Task.CompletedTask;
        }

        private async Task OnReactionAdd(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction sReaction)
        {
            IUserMessage message = await cache.GetOrDownloadAsync();
            ITextChannel textChannel = (ITextChannel) channel;

            var player = _lavaSocketClient.GetPlayer(textChannel.Guild.Id);

            if (player == null)
            {
                return;
            }

            foreach (Embed embed in message.Embeds)
            {
                string[] key = embed.Title.Split(" ");

                if (sReaction.UserId != _client.CurrentUser.Id)
                {
                    if (key[0] == "queue:")
                    {
                        await message.RemoveAllReactionsAsync(new RequestOptions());

                        int p = int.Parse(key[3]);

                        int forward = p + 10;
                        int fTrackPos = forward - 9;
                        int fTrackStart = forward - 9;

                        int prev = p - 10;
                        int pTrackPos = prev - 9;
                        int pTrackStart = prev -9;

                        if (sReaction.Emote.Name == "➡")
                        {
                            getQueueResults(player, textChannel, fTrackPos, fTrackStart, forward, message, false);
                        }

                        if (sReaction.Emote.Name == "⬅")
                        {
                            getQueueResults(player, textChannel, pTrackPos, pTrackStart, prev, message, false);
                        }
                    }
                }
            }
        }

        public async Task ConnectAsync(SocketVoiceChannel voiceChannel, ITextChannel channel)
        {
            if (voiceChannel == null)
            {
                return;
            }
            else
            {
                await _lavaSocketClient.ConnectAsync(voiceChannel, channel);
                return;
            }
        }

        public async Task<Embed> LeaveAsync(SocketVoiceChannel voiceChannel, ulong guildId)
        {
            var player = _lavaSocketClient.GetPlayer(guildId);
            var embed = new EmbedBuilder();
            if (player == null)
            {
                embed.WithDescription("Cannot leave a voice channel if not in one!");
                return embed.Build();
            }
            else
            {
                await _lavaSocketClient.DisconnectAsync(voiceChannel);
                embed.WithDescription($"Disconnected from: {voiceChannel.Name}");
                return embed.Build();
            }

        }

        public async Task<Embed> PlayAsync(string query, ulong guildId)
        {
            var player = _lavaSocketClient.GetPlayer(guildId);
            var results = await _lavaRestClient.SearchYouTubeAsync(query);
            var embed = new EmbedBuilder();
            if (results.LoadType == LoadType.NoMatches || results.LoadType == LoadType.LoadFailed)
            {
                embed.WithDescription("No songs matched your search query!");
                return embed.Build();
            }

            var track = results.Tracks.FirstOrDefault();

            if (player.IsPlaying)
            {
                player.Queue.Enqueue(track);
                embed.WithDescription($"[{track.Title}]({track.Uri}) has been added to the queue!");
                return embed.Build();
            }
            else
            {
                await player.PlayAsync(track);
                embed.WithDescription($"Now playing: [{track.Title}]({track.Uri})");
                return embed.Build();
            }
        }
        public async Task PlaylistAsync(List<string> query, ulong guildId, ITextChannel textChannel)
        {
            LavaPlayer player = _lavaSocketClient.GetPlayer(guildId);
            StringBuilder descriptionBuilder = new StringBuilder();
            EmbedBuilder embed = new EmbedBuilder();

            embed.WithTitle($"Adding {query.Count} Tracks to queue from playlist!");

            if (!player.IsPlaying)
            {
                SearchResult first = await _lavaRestClient.SearchYouTubeAsync(query[0]);
                LavaTrack track1 = first.Tracks.FirstOrDefault();
                await player.PlayAsync(track1);

                int trackPos = 2;

                var message = await textChannel.SendMessageAsync(null, false, embed.Build());
                foreach (var tracks in query.Skip(1))
                {
                    SearchResult results = await _lavaRestClient.SearchYouTubeAsync(tracks);
                    LavaTrack track = results.Tracks.FirstOrDefault();
                    descriptionBuilder.Append($"{trackPos}: [{track.Title}]({track.Uri}) - [{track.Length}]\n\n");
                    player.Queue.Enqueue(track);

                    embed.WithDescription($"1: [{track1.Title}]({track1.Uri}) - [{track1.Length}]\n\n{descriptionBuilder.ToString()}");
                    await message.ModifyAsync(m => {
                        m.Embed = embed.Build();
                    });

                    trackPos++;
                }
            } 
            else
            {
                int trackPos = 1;

                var message = await textChannel.SendMessageAsync(null, false, embed.Build());

                foreach (var tracks in query)
                {
                    SearchResult results = await _lavaRestClient.SearchYouTubeAsync(tracks);
                    LavaTrack track = results.Tracks.FirstOrDefault();
                    descriptionBuilder.Append($"{trackPos}: [{track.Title}]({track.Uri}) - [{track.Length}]\n\n");
                    player.Queue.Enqueue(track);

                    embed.WithDescription($"{descriptionBuilder.ToString()}");
                    await message.ModifyAsync(m => {
                        m.Embed = embed.Build();
                    });

                    trackPos++;
                }
            }
        }
        public async Task<Embed> StopAsync(ulong guildId)
        {
            var player = _lavaSocketClient.GetPlayer(guildId);
            var embed = new EmbedBuilder();
            if (player is null)
            {
                embed.WithDescription("No playback found, cannot stop!");
                return embed.Build();
            }
            else
            {
                await player.StopAsync();
                embed.WithDescription("Music playback stopped!");
                return embed.Build();
            }

        }

        public async Task<Embed> SkipAsync(ulong guildId)
        {
            var player = _lavaSocketClient.GetPlayer(guildId);
            var embed = new EmbedBuilder();
            if (player is null || player.Queue.Items.Count() is 0)
            {
                embed.WithDescription("Nothing in queue to skip!");
                return embed.Build();
            }
            else
            {
                var track = player.CurrentTrack;
                await player.SkipAsync();
                embed.WithDescription($"Skipped {track.Title}:\nnow playing: {player.CurrentTrack.Title}");
                return embed.Build();
            }
        }

        public async Task<Embed> SetVolumeAsync(int vol, ulong guildId)
        {
            var player = _lavaSocketClient.GetPlayer(guildId);
            var embed = new EmbedBuilder();
            if (player is null)
            {
                return default;
            }

            if (vol >= 150 || vol <= 2)
            {
                embed.WithDescription("Volume value out of range! `min=2`, `max=150`");
                return embed.Build();
            }
            else
            {
                await player.SetVolumeAsync(vol);
                embed.WithDescription($"Ajusted Volume to {vol}");
                return embed.Build();
            }
        }

        public async Task<Embed> PauseAsync(ulong guildId)
        {
            var player = _lavaSocketClient.GetPlayer(guildId);
            var embed = new EmbedBuilder();
            if (player is null)
            {
                embed.WithDescription("Cannot pause if music is not playing");
                return embed.Build();
            }
            if (!player.IsPaused)
            {
                await player.PauseAsync();
                embed.WithDescription("Paused playback!");
                return embed.Build();
            }
            else
            {
                await player.ResumeAsync();
                embed.WithDescription("Playback resumed!");
                return embed.Build();
            }

        }

        public async Task<Embed> ResumeAsync(ulong guildId)
        {
            var player = _lavaSocketClient.GetPlayer(guildId);
            var embed = new EmbedBuilder();
            if (player is null) return default;
            if (player.IsPaused)
            {
                await player.ResumeAsync();
                embed.WithDescription("Playback resumed!");
                return embed.Build();
            }
            else
            {
                embed.WithDescription("Playback is not paused; cannot resume an already playing song!");
                return embed.Build();
            }
        }

        public async Task QueueAsync(ulong guildId, ITextChannel textChannel)
        {

            var descriptionBuilder = new StringBuilder();
            var player = _lavaSocketClient.GetPlayer(guildId);
            var embed = new EmbedBuilder();
            embed.WithTitle("queue:");
            embed.WithDescription("");

            if (player == null)
            {
                embed.WithDescription("\n - Nothing in Queue");
                await textChannel.SendMessageAsync(null, false, embed.Build());
            }
            else
            {
                if (player.IsPlaying)
                {
                    if (player.Queue.Count < 1 && player.CurrentTrack != null)
                    {
                        embed.WithDescription($"__Now Playing__:\n - [{player.CurrentTrack.Title}]({player.CurrentTrack.Uri}) - [{player.CurrentTrack.Length}]");
                        embed.WithFooter($"Nothing else in queue! current runtime = [{player.CurrentTrack.Length}]");
                        await textChannel.SendMessageAsync(null, false, embed.Build());
                    }
                    else
                    {
                        var message = await textChannel.SendMessageAsync(null, false, embed.Build());
                        getQueueResults(player, textChannel, 1, 1, 10, message, false);
                    }
                }
                else
                {
                    embed.WithDescription("Player is not playing anything!");
                    await textChannel.SendMessageAsync(null, false, embed.Build());
                }
            }
        }

        private async void getQueueResults(LavaPlayer player, ITextChannel textChannel, int trackPos, int startPos, int end, IUserMessage message, bool page = false)
        {
            var result = new List<IQueueObject>();
            var descriptionBuilder = new StringBuilder();
            EmbedBuilder embed = new EmbedBuilder();

            embed.WithTitle($"queue: {startPos} - {end}");
            embed.WithDescription($":arrows_counterclockwise: Loading {player.Queue.Count} queue results! May take some time...");
            await message.ModifyAsync(m => {
                m.Embed = embed.Build();
            });

            foreach (var tracks in player.Queue.Items.Skip(startPos-2))
            {
                result.Add(tracks);
            }

            foreach (var tracks in result)
            {
                LavaTrack track = (LavaTrack)tracks;

                if (trackPos <= end)
                {
                    descriptionBuilder.Append($"{trackPos}: [{track.Title}]({track.Uri}) - [{track.Length}]\n\n");
                    embed.WithTitle($"queue: {startPos} - {end}");
                    trackPos++;
                }
            }

            embed.WithDescription($"__Now Playing__:\n - " +
                $"[{player.CurrentTrack.Title}]({player.CurrentTrack.Uri}) - " +
                $"[{player.CurrentTrack.Length}]" +
                $"\n\n:arrow_double_down: __Up Next__::arrow_double_down:" +
                $"\n\n{descriptionBuilder.ToString()}");

            if (trackPos >= player.Queue.Count && trackPos > 11)
            {
                IEmote left = new Emoji("⬅");
                await message.AddReactionAsync(left);
            } 
            
            if (trackPos <= player.Queue.Count)
            {
                IEmote right = new Emoji("➡");
                await message.AddReactionAsync(right);
            }

            embed.WithFooter($"{player.Queue.Count} Songs in queue!");
            await message.ModifyAsync(m => {
                m.Embed = embed.Build();
            });
        }

        public async Task<LavaTrack> GetTrack(string query, ulong guildId)
        {
            var player = _lavaSocketClient.GetPlayer(guildId);
            var results = await _lavaRestClient.SearchYouTubeAsync(query);
            var embed = new EmbedBuilder();
            return results.Tracks.FirstOrDefault();
        }

        private async Task OnTrackFinished(LavaPlayer player, LavaTrack track, TrackEndReason reason)
        {
            if (ConfigService.GetKey(player.TextChannel.GuildId))
            {
                player.Queue.Enqueue(track);
            }

            var embed = new EmbedBuilder();
            if (!reason.ShouldPlayNext()) return;
            if (!player.Queue.TryDequeue(out var item) || !(item is LavaTrack nextTrack))
            {
                embed.WithDescription("There are no more tracks in the queue!");
                await player.TextChannel.SendMessageAsync(null, false, embed.Build());
                return;
            }
            else
            {
                embed.WithDescription($"Now playing: [{nextTrack.Title}]({nextTrack.Uri})");
                await player.TextChannel.SendMessageAsync(null, false, embed.Build());
                await player.PlayAsync(nextTrack);
            }


        }

        private Task LogAsync(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }

        private async Task ClientReadyAsync()
        {
            await _lavaSocketClient.StartAsync(_client, new Configuration { 
                Host = "18.223.252.0",
                Password = "18ReVeltIon"
            });
        }
    }
}
