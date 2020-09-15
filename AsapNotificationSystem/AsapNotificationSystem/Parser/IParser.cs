using System;
using System.Collections.Generic;
using System.Text;

namespace AsapNotificationSystem.Parser
{
    public interface IParser<I, O>
    {
        O Parse(I inputMessage);
    }
}
