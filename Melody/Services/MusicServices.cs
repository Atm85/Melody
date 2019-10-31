using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Entities;

namespace Melody.Services
{
    public class MusicService
    {
        private DiscordSocketClient _client;
        private LavaRestClient _lavaRestClient;
        private LavaSocketClient _lavaSocketClient;
        private LavaPlayer _player;

        public MusicService(DiscordSocketClient client, LavaRestClient lavaRestClient, LavaSocketClient lavaSocketClient)
        {
            _client = client;
            _lavaRestClient = lavaRestClient;
            _lavaSocketClient = lavaSocketClient;
        }

        public Task InitializeAsync()
        {
            _client.Ready += ClientReadyAsync;
            _lavaSocketClient.Log += LogAsync;
            _lavaSocketClient.OnTrackFinished += OnTrackFinished;
            return Task.CompletedTask;
        }

        public async Task<Embed> ConnectAsync(SocketVoiceChannel voiceChannel, ITextChannel channel)
        {
            var embed = new EmbedBuilder();

            if (voiceChannel == null)
            {
                embed.WithDescription("You are not connected to a voice channel!");
                return embed.Build();
            }
            else
            {
                await _lavaSocketClient.ConnectAsync(voiceChannel, channel);
                embed.WithDescription($"Now connected to: {voiceChannel.Name}");
                return embed.Build();
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

        public async Task<Embed> QueueAsync(ulong guildId)
        {

            var descriptionBuilder = new StringBuilder();
            var player = _lavaSocketClient.GetPlayer(guildId);
            var embed = new EmbedBuilder();
            embed.WithTitle("Music Playlist!");

            if (player == null)
            {
                embed.WithDescription("\n - Nothing in Queue");
                return embed.Build();
            }
            else
            {
                if (player.IsPlaying)
                {
                    if (player.Queue.Count < 1 && player.CurrentTrack != null)
                    {
                        embed.WithDescription($"__Now Playing__:\n - [{player.CurrentTrack.Title}]({player.CurrentTrack.Uri}) - [{player.CurrentTrack.Length}]");
                        embed.WithFooter($"Nothing else in queue! current runtime = [{player.CurrentTrack.Length}]");
                        return embed.Build();
                    }
                    else
                    {
                        var trackPos = 1;
                        var h = 00;
                        var m = 00;
                        var s = 00;
                        foreach (var tracks in player.Queue.Items)
                        {
                            LavaTrack track = (LavaTrack)tracks;
                            descriptionBuilder.Append($"{trackPos}: [{track.Title}]({track.Uri}) - [{track.Length}]\n\n");

                            h += track.Length.Hours;
                            m += track.Length.Minutes;
                            s += track.Length.Seconds;

                            if (s >= 60)
                            {
                                m += 1;
                                s = 0;
                            }

                            if (m >= 60)
                            {
                                h += 1;
                                m = 0;
                            }

                            trackPos++;
                            
                        }

                        var runtime = h + ":" + m + ":" + s;

                        embed.WithTitle("Music Playlist!");
                        embed.WithDescription($"__Now Playing__:\n - " +
                            $"[{player.CurrentTrack.Title}]({player.CurrentTrack.Uri}) - " +
                            $"[{player.CurrentTrack.Length}]" +
                            $"\n\n:arrow_double_down: __Up Next__::arrow_double_down:" +
                            $"\n\n{descriptionBuilder.ToString()}");
                        embed.WithFooter($"{trackPos} Songs in queue! current runtime = [{runtime}]");
                        return embed.Build();
                    }
                }
                else
                {
                    embed.WithDescription("Player is not playing anything!");
                    return embed.Build();
                }
            }
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
