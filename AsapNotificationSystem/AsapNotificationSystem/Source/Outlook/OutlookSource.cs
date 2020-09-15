using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using AsapNotificationSystem.Parser;
using AsapNotificationSystem.Source.Outlook.Config;

using MailKit.Net.Pop3;
using MimeKit;

namespace AsapNotificationSystem.Source.Outlook
{
    public class OutlookSource : ISource, IDisposable
    {
        static string host = "outlook.office365.com";
        static int port = 995;

        public event Action<IEnumerable<string>, IEnumerable<object>> NewEvent;

        public bool Pause
        {
            get => config.Pause; 
            set => config.Pause = value; 
        }

        private OutlookConfig config;

        private IParser<MimeMessage, bool> parser;
        private Pop3Client client;
        private Task checkTask;

        public OutlookSource(string login, string password, IParser<MimeMessage, bool> messageParser, DateTime? emailSinceDate = null, bool start = true) 
            :this(new OutlookConfig
            {
                Login = login,
                Password = password,
                Pause = !start,
                Time = DateTime.MinValue,
                Path = "outlookConfig.json"
            }, messageParser) { }

        public OutlookSource(OutlookConfig config, IParser<MimeMessage, bool> messageParser)
        {
            this.config = config;

            host = config.Host;
            port = config.Port;

            client = new Pop3Client();
            client.Connect(host, port);
            client.Authenticate(config.Login, config.Password);

            Pause = config.Pause;

            parser = messageParser;
            checkTask = Task.Factory.StartNew(CheckNewEmails);
        }

        public void Dispose()
        {
            client.Disconnect(true);
            checkTask.Dispose();
        }

        private async void CheckNewEmails()
        {
            var emails = new List<MimeMessage>();
            while (true)
            {
                if (!Pause)
                {
                    Console.WriteLine(emails.Count);
                    GetNewMessages(emails);
                    Console.WriteLine(emails.Count);

                    if (emails.Any())
                    {
                        Console.WriteLine(emails.Where(parser.Parse).Count());
                        emails.AsParallel().Where(parser.Parse).ForAll(x =>
                        {
                            var path = $"{new Random().Next(0, 100000000)}_{x.Attachments.First().ContentDisposition.FileName}";
                            using (var fs = File.Create(path))
                                SaveAttachment(x.Attachments.First(), fs);
                            Task.Factory.StartNew(() =>
                            {
                                NewEvent?.Invoke(new[] {"OLV1", "file", path }, null);
                            });
                        });
                    }
                }

                await Task.Delay(1000);
            }
        }

        private void GetNewMessages(List<MimeMessage> emailsList)
        {
            Console.WriteLine(":1");
            if (emailsList == null)
            {
                emailsList = new List<MimeMessage>();
            }
            else
            {
                emailsList.Clear();
            }

            var count = client.Count;
            Console.WriteLine(":2");
            if (count == 0)
            {
                config.Time = DateTime.Now;
            }
            else
            {
                if (config.Time == DateTime.MaxValue)
                {
                    config.Time = client.GetMessage(count - 1).Date.LocalDateTime;
                    emailsList.AddRange(client);
                }
                else
                {
                    Console.WriteLine(config.Time);
                    Console.WriteLine(client.GetMessage(count - 1).Date.LocalDateTime);
                    Console.WriteLine(client.GetMessage(0).Date.LocalDateTime);
                    for (int i = count - 1; i >= 0; i--)
                    {
                        var temp = client.GetMessage(i);
                        if (temp.Date.LocalDateTime > config.Time)
                        {
                            emailsList.Add(temp);
                        }
                        else
                        {
                            break;
                        }
                    }
                    Console.WriteLine(":3");
                    if (emailsList.Any())
                    {
                        config.Time = emailsList[0].Date.LocalDateTime;
                        emailsList.Reverse();
                    }
                }
            }
            Console.WriteLine(":4");
        }

        public static void SaveAttachment(MimeEntity attachment, FileStream fs)
        {
            if (attachment is MessagePart mesPart)
            {
                mesPart.Message.WriteTo(fs);
            }
            else
            {
                var mimePart = (MimePart)attachment;
                mimePart.Content.DecodeTo(fs);
            }
        }
    }
}
