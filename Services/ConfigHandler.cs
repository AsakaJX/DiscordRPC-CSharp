using Newtonsoft.Json;

namespace AsakaDiscordRPC.Services {
    public class Config {
        public static string Read(string filePath) {
            StreamReader reader = new(filePath);
            string json = reader.ReadToEnd();
            return json;
        }
        public static dynamic GetConfig() {
            string json = Read(@"config.json");
            dynamic? array = JsonConvert.DeserializeObject(json) ?? throw new Exception("array is null");
            return array;
        }
    }
}
