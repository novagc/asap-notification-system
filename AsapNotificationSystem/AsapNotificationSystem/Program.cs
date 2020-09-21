using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AsapNotificationSystem.Bot;
using AsapNotificationSystem.Bot.Config;
using AsapNotificationSystem.Bot.Keyboard;
using AsapNotificationSystem.Bot.Keyboard.Vk;
using AsapNotificationSystem.Bot.Vk;
using AsapNotificationSystem.ConfigReaderService;
using AsapNotificationSystem.Convert;
using AsapNotificationSystem.DataBase;
using AsapNotificationSystem.DataBase.Models;
using AsapNotificationSystem.Message;
using AsapNotificationSystem.Parser;
using AsapNotificationSystem.Source;
using AsapNotificationSystem.Source.Email;
using AsapNotificationSystem.Source.Email.Config;
using Newtonsoft.Json;

namespace AsapNotificationSystem
{
    public class Program
    {
        public static DataBaseService DataBase;

        public static IEnumerable<ISenderBot<string>> SenderBots;
        public static IEnumerable<IReceiverBot<string[]>> ReceiverBots;
        public static IEnumerable<ISource> Sources;

        private static List<MessageModel> _messages = new List<MessageModel>();
 
        private static VkKeyboard keyboard = new VkKeyboard(Enumerable.Range(0, 39)
            .Select((x) =>
            {
                var temp = BnToString.Convert((BuildingNumber) x);
                return new VkButton(temp, "positive", temp);
            }));

        static void Main(string[] args)
        {
            Init();

            Console.WriteLine("Start!");

            while (true)
            {
                Console.ReadLine();
            }
        }

        private static void Init()
        {
            var temp = new BasicConfigReader<A>("configs/mesConfig.json").ParseConfig();
            _messages = temp.Messages;

            DataBase = new DataBaseService("configs/dbConfig.json");
            DataBase.NewUser += NewUser;

            var vkBot = new VkMultiBot(new BasicConfigReader<VkBotConfig>("configs/vkBotConfig.json").ParseConfig());

            SenderBots = new List<ISenderBot<string>>
            {
                vkBot
            };

            ReceiverBots = new List<IReceiverBot<string[]>>
            {
                vkBot
            };

            var oc =
                new BasicConfigReader<EmailConfig>("configs/emailConfig.json").ParseConfig();
            oc.Path = "configs/emailConfig.json";

            Sources = new List<ISource>
            {
                new EmailSource(
                    oc,
                    new EmailParser(null, null, true, new []{"отключение.xls", "отключение.xlsx"}))
            };

            ReceiverBots.First().NewMessage += (_, data) =>
            {
                Console.WriteLine("!");
                var text = "Меню";
                var kb = keyboard.Clone();
                var temp = StringToBnConverter.Convert(data[0]);
                Console.WriteLine("!!");
                if (temp != BuildingNumber.Other)
                {
                    DataBase.ChangeBuildingNumber(0, data[1], temp);
                    text = "Ок";
                } 
                    
                foreach (var bn in DataBase.GetUserBn(0, data[1]))
                {
                    ((VkButton) (kb.Buttons[System.Convert.ToInt32(bn)])).Color = "negative";
                }
                Console.WriteLine("!!!");
                SenderBots.First().SendMessage(text, data[1], kb);
            };

            Sources.First().NewEvent += (a, b) =>
            {
                var tPath = a.Skip(2).First();
                var mess = new ExcelParser().Parse(tPath);
                
                _messages.AddRange(mess);
                File.WriteAllText("configs/mesConfig.json", JsonConvert.SerializeObject(new A{Messages = _messages}));
                
                mess.AsParallel().ForAll(x =>
                {
                    var temp = DataBase.SelectUsersByBuildingNumber(x.BuildingNumber);
                    
                    SenderBots.First().SendMassMessage(x.BuildingNumber == BuildingNumber.All
                        ? $"Во всех корпусах c {x.From} по {x.To} будут отключены: {x.What}"
                        : $"В корпусе {BnToString.Convert(x.BuildingNumber)} c {x.From} по {x.To} будут отключены: {x.What}", temp);
                });
            };
        }

        private static void NewUser(User user)
        {
            _messages.AsParallel().ForAll(x =>
            {
                if (x.TimeTo >= DateTime.Now && (x.BuildingNumber == BuildingNumber.All || user.Number.Contains(x.BuildingNumber)))
                {
                    SenderBots.First().SendMessage(
                        x.BuildingNumber == BuildingNumber.All
                            ? $"Во всех корпусах c {x.From} по {x.To} будут отключены: {x.What}"
                            : $"В корпусе {BnToString.Convert(x.BuildingNumber)} c {x.From} по {x.To} будут отключены: {x.What}",
                        user.ProfileId);
                }
            });
        }
    }

    public class A
    {
        public List<MessageModel> Messages { get; set; }
    }
}
