using System;
using System.IO;
using Newtonsoft.Json;

namespace AsapNotificationSystem.Source.Email.Config
{
    public class EmailConfig
    {
        [JsonIgnore]
        private DateTime time;

        [JsonIgnore]
        private bool pause;

        public string Host { get; set; }

        public int Port { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public DateTime Time
        {
            get => time;
            set
            {
                time = value;
                Save();
            }
        }

        public bool Pause
        {
            get => pause;
            set
            {
                pause = value;
                Save();
            }
        }

        [JsonIgnore]
        public string Path { get; set; }

        private void Save()
        {
            if (Path != null)
            {
                lock(Path)
                {
                    File.WriteAllText(Path, JsonConvert.SerializeObject(this));
                }
            }
        }
    }
}
