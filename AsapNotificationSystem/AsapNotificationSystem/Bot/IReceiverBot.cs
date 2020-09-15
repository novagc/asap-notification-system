using System;
using System.Collections.Generic;
using System.Text;
using AsapNotificationSystem.Bot.Config;

namespace AsapNotificationSystem.Bot
{
    public interface IReceiverBot<T> where T: class
    {
        int ServiceId { get; }
        IBotConfig ReceiverBotConfig { get; }

        void Init();

        event EventHandler<T> NewMessage;

        event EventHandler<string> StartDialog;
        event EventHandler<string> EndDialog;
    }
}