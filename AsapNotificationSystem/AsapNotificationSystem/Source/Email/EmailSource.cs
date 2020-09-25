using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using AsapNotificationSystem.Parser;
using AsapNotificationSystem.Source.Email.Config;
using MailKit.Net.Pop3;
using MimeKit;

namespace AsapNotificationSystem.Source.Email
{
    public class EmailSource : ISource, IDisposable
    {
        public event Action<IEnumerable<string>, IEnumerable<object>> NewEvent;

        public bool Pause
        {
            get => config.Pause; 
            set => config.Pause = value; 
        }

        string host;
        int port;

        private EmailConfig config;

        private IParser<MimeMessage, bool> parser;
        private Task checkTask;

        public EmailSource(string login, string password, IParser<MimeMessage, bool> messageParser, DateTime? emailSinceDate = null, bool start = true) 
            :this(new EmailConfig
            {
                Login = login,
                Password = password,
                Pause = !start,
                Time = DateTime.MinValue,
                Path = "configs/emailConfig.json"
            }, messageParser) { }

        public EmailSource(EmailConfig config, IParser<MimeMessage, bool> messageParser)
        {
            this.config = config;

            host = config.Host;
            port = config.Port;

            Pause = config.Pause;

            parser = messageParser;
            checkTask = Task.Factory.StartNew(CheckNewEmails);
        }

        public void Dispose()
        {
            checkTask.Dispose();
        }

        private async void CheckNewEmails()
        {
            var emails = new List<MimeMessage>();
            while (true)
            {
                if (!Pause)
                {
                    try
                    {
                        GetNewMessages(emails);

                        if (emails.Any())
                        {
                            emails.AsParallel().Where(parser.Parse).ForAll(x =>
                            {
                                var path =
                                    $"{new Random().Next(0, 100000000)}_{x.Attachments.First().ContentDisposition.FileName}";
                                using (var fs = File.Create(path))
                                    SaveAttachment(x.Attachments.First(), fs);
                                Task.Factory.StartNew(() => { NewEvent?.Invoke(new[] {"OLV1", "file", path}, null); });
                            });
                        }
                    }
                    catch { }
                }

                await Task.Delay(1000*60*30);
            }
        }

        private void GetNewMessages(List<MimeMessage> emailsList)
        {
            using var client = new Pop3Client();
            client.Connect(host, port);
            client.Authenticate(config.Login, config.Password);

            if (emailsList == null)
            {
                emailsList = new List<MimeMessage>();
            }
            else
            {
                emailsList.Clear();
            }

            var count = client.Count;
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
                    Console.WriteLine(client.GetMessage(0).Date.LocalDateTime);
                    for (int i = 0; i < count; i++)
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
                    if (emailsList.Any())
                    {
                        config.Time = emailsList[0].Date.LocalDateTime;
                        emailsList.Reverse();
                    }
                }
            }
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
