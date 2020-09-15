using System;
using System.Collections.Generic;
using System.Text;
using AsapNotificationSystem.Bot.Config;
using AsapNotificationSystem.Bot.Keyboard;

namespace AsapNotificationSystem.Bot
{
    public interface ISenderBot<T> where T: class
    {
        int ServiceId { get; }
        IBotConfig SenderBotConfig { get; }

        void Init();

        void SendMessage(T message, string chatId, IKeyboard keyboard = null);
        void SendMassMessage(T message, IEnumerable<string> chatIDs);
    }
}
