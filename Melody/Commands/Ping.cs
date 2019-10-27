using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Melody.Commands
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Alias("latency")]
        public async Task PingAsync()
        {
            var timer = Stopwatch.StartNew();
            var message = await ReplyAsync($"**Websocket latency**: {Context.Client.Latency}ms\n**Responce**: ...");
            timer.Stop();

            await message.ModifyAsync(message => {
                message.Content = $"**Websocket latency**: {Context.Client.Latency}ms\n **Response**: {timer.Elapsed.TotalMilliseconds}ms";
        });
        }
    }
}
