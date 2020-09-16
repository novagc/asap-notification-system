using System;
using System.Collections.Generic;
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
using AsapNotificationSystem.Parser;
using AsapNotificationSystem.Source;
using AsapNotificationSystem.Source.Outlook;
using AsapNotificationSystem.Source.Outlook.Config;
using Newtonsoft.Json;

namespace AsapNotificationSystem
{
    public class Program
    {
        public static DataBaseService DataBase;

        public static IEnumerable<ISenderBot<string>> SenderBots;
        public static IEnumerable<IReceiverBot<string[]>> ReceiverBots;
        public static IEnumerable<ISource> Sources;

        private static VkKeyboard keyboard = new VkKeyboard(Enumerable.Range(0, 36)
            .Select((x) =>
            {
                var temp = BnToString.Convert((BuildingNumber) x);
                return new VkButton(temp, "positive", temp);
            }));

        static void Main(string[] args)
        {
            Init();

            Console.WriteLine("Start!");

            while(true)
            {
                Console.ReadLine();
            }
        }

        private static void Init()
        {
            DataBase = new DataBaseService("configs/dbConfig.json");

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
                new BasicConfigReader<OutlookConfig>("configs/outlookConfig.json").ParseConfig();
            oc.Path = "configs/outlookConfig.json";

            Sources = new List<ISource>
            {
                new OutlookSource(
                    oc,
                    new EmailParser(new []{ "novagc@yandex.ru" }, null, true))
            };

            ReceiverBots.First().NewMessage += (_, data) =>
            {
                var text = "Меню";
                var kb = keyboard.Clone();

                if (data[2] != null)
                {
                    var pl = JsonConvert.DeserializeObject<Payload>(data[2]);
                    Console.WriteLine(pl.button);
                    var num = StringToBnConverter.Convert(pl.button);
                    DataBase.ChangeBuildingNumber(0, data[1], num);
                    text = "Ок";
                }

                foreach (var bn in DataBase.GetUserBn(0, data[1]))
                {
                    ((VkButton) (kb.Buttons[System.Convert.ToInt32(bn)])).Color = "negative";
                }
                SenderBots.First().SendMessage(text, data[1], kb);
            };

            Sources.First().NewEvent += (a, b) =>
            {
                var tPath = a.Skip(2).First();
                var mess = new ExcelParser().Parse(tPath);
 
                mess.AsParallel().ForAll(x =>
                {
                    var temp = DataBase.SelectUsersByBuildingNumber(x.BuildingNumber);
                    
                    SenderBots.First().SendMassMessage($"В корпусе {BnToString.Convert(x.BuildingNumber)} c {x.From} по {x.To} будут отключены: {x.What}", temp);
                });
            };
        }
    }

    public class Payload
    {
        public string button { get; set; }
    }
}
