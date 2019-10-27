using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Melody.Services
{
    class TokenService
    {

        private const string folder = "Resources";
        private const string config = "token.json";

        public static BotConfig bot;

        static TokenService()
        {
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            if (!File.Exists(folder + "/" + config))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(folder + "/" + config, json);
            }
            else
            {
                string json = File.ReadAllText(folder + "/" + config);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }
    }

    public struct BotConfig
    {
        public string token;
        public string prefix;
    }
}
