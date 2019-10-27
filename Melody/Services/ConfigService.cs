using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Melody.Services
{
    class ConfigService
    {
        private const string path = "Resources";
        private const string file = "config.json";

        public static void CreateDefaultDictionary(ulong guildId)
        {
            Dictionary<ulong, Dictionary<string, bool>> dictionary = new Dictionary<ulong, Dictionary<string, bool>>();
            Dictionary<string, bool> value = new Dictionary<string, bool>();
            value.Add("repeat", false);
            dictionary.Add(guildId, value);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(path + "/" + file))
            {
                string json = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
                File.WriteAllText(path + "/" + file, json);
            }
            else
            {
                string json = File.ReadAllText(path + "/" + file);
                dictionary = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<string, bool>>>(json);
                if (!dictionary.ContainsKey(guildId))
                {
                    dictionary.Add(guildId, value);
                    string jsonData = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
                    File.WriteAllText(path + "/" + file, jsonData);
                }
            }
        }
        public static void UpdateDictionary(ulong guildId, bool v)
        {
            string jsonString = File.ReadAllText(path + "/" + file);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<string, bool>>>(jsonString);
            var value = new Dictionary<string, bool>();
            if (dictionary.ContainsKey(guildId))
            {
                value.Add("repeat", v);
                dictionary[guildId] = value;
                string jsonData = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
                File.WriteAllText(path + "/" + file, jsonData);
            }
        }
        public static bool GetKey(ulong guildId)
        {
            string jsonString = File.ReadAllText(path + "/" + file);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<string, bool>>>(jsonString);
            return dictionary[guildId]["repeat"];
        }
    }
}
