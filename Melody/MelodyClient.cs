using Melody.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Victoria;

namespace Melody
{
    class MelodyClient
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;

        private IServiceProvider _serviceProvider;

        public MelodyClient(DiscordSocketClient client = null, CommandService commandService = null)
        {
            _client = client ?? new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 50,
                LogLevel = LogSeverity.Debug
            });
            _commandService = commandService ?? new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false
            });
        }

        public async Task InitializeAsync()
        {
            Console.WriteLine("Starting c# bot");
            var token = TokenService.bot.token;

            if (token != null)
            {
                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();
                await _client.SetGameAsync($"Music | {TokenService.bot.prefix}help", null, ActivityType.Listening);

                _client.Log += LogAsync;
                _client.JoinedGuild += MelodyBotJoin;
                _serviceProvider = ServiceProvider();

                var commandHandler = new CommandHandler(_client, _commandService, _serviceProvider);

                await _serviceProvider.GetRequiredService<MusicService>().InitializeAsync();
                await commandHandler.InitializeAsync();
                await Task.Delay(-1);
            } else
            {
                Console.WriteLine("No token detected!");
                new TokenService();
            }
            
        }

        private Task MelodyBotJoin(SocketGuild arg)
        {
            ConfigService.CreateDefaultDictionary(arg.Id);
            return Task.CompletedTask;
        }

        private Task LogAsync(LogMessage logMessage)
        {
            Console.WriteLine(logMessage);
            return Task.CompletedTask;
        }

        private IServiceProvider ServiceProvider()
        {
            return new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commandService)
            .AddSingleton(new LavaRestClient(new Configuration { Password = "18ReVeltIon", Host = "18.223.252.0" }))
            .AddSingleton<LavaSocketClient>()
            .AddSingleton<MusicService>()
            .BuildServiceProvider();
        }
    }
}
