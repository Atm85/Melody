using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Melody
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;

        public CommandHandler(DiscordSocketClient client, CommandService command, IServiceProvider service)
        {
            _client = client;
            _commandService = command;
            _serviceProvider = service;
        }

        public async Task InitializeAsync()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
            _commandService.Log += LogAsync;
            _client.MessageReceived += MessageReceived;
        }

        private async Task MessageReceived(SocketMessage message)
        {
            var args = 0;
            var userMessage = message as SocketUserMessage;

            if (userMessage == null) return;
            if (!(message is SocketUserMessage)) return;
            if (userMessage.HasMentionPrefix(_client.CurrentUser, ref args) || userMessage.HasStringPrefix(".", ref args))
            {
                var context = new SocketCommandContext(_client, userMessage);
                var result = await _commandService.ExecuteAsync(context, args, _serviceProvider);
            }
        }

        private Task LogAsync(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}
