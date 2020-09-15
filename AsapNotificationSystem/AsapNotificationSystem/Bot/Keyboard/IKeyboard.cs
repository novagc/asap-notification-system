using System;
using System.Collections.Generic;
using System.Text;

namespace AsapNotificationSystem.Bot.Keyboard
{
    public interface IKeyboard
    {
        List<IButton> Buttons { get; }
    }
}
