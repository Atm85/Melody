using Discord;
using Discord.Commands;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Alias("latency")]
        public async Task PingAsync()
        {
            var textChannel = Context.Channel as ITextChannel;
            await textChannel.TriggerTypingAsync();
            var timer = Stopwatch.StartNew();
            var message = await ReplyAsync($"**Websocket latency**: {Context.Client.Latency}ms\n**Responce**: ...");
            timer.Stop();

            await message.ModifyAsync(message => {
                message.Content = $"**Websocket latency**: {Context.Client.Latency}ms\n **Response**: {timer.Elapsed.TotalMilliseconds}ms";
        });
        }
    }
}
