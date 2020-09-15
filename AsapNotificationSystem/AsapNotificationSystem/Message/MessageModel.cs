using System;
using System.Collections.Generic;
using System.Text;
using AsapNotificationSystem.DataBase.Models;

namespace AsapNotificationSystem.Message
{
    public class MessageModel
    {
        public BuildingNumber BuildingNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string What { get; set; }
    }
}
