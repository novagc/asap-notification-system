using System;
using System.Collections.Generic;
using System.Text;

namespace AsapNotificationSystem.Source
{
    public interface ISource
    {
        event Action<IEnumerable<string>, IEnumerable<object>> NewEvent;
        bool Pause { get; set; }
    }
}
