using System.IO;
using Newtonsoft.Json;

namespace AsapNotificationSystem.ConfigReaderService
{
    public class BasicConfigReader<T> : IConfigReader<T> where T : class
    {
        public string ConfigPath { get; set; }

        public BasicConfigReader(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Config {path} not found");
            }

            ConfigPath = path;
        }

        public T ParseConfig()
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(ConfigPath));
        }
    }
}
