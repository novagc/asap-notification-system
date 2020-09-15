using System;
using System.Collections.Generic;
using System.Text;

namespace AsapNotificationSystem.Bot.Config
{
    public class VkBotConfig : IBotConfig
    {
        public string Token { get; set; }
        public int WaitTime { get; set; }
        public int PauseTime { get; set; }
        public bool WriteLogs { get; set; }
        public ulong GroupId { get; set; }
    }
}
