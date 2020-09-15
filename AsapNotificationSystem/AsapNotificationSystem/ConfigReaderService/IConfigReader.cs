using System;
using System.Collections.Generic;
using System.Text;

namespace AsapNotificationSystem.ConfigReaderService
{
    public interface IConfigReader<T> where T : class
    {
        string ConfigPath { get; set; }

        T ParseConfig();
    }
}
