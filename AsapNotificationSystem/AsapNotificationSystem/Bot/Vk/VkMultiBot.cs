using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;

using AsapNotificationSystem.Bot.Config;
using AsapNotificationSystem.Bot.Keyboard;
using AsapNotificationSystem.Bot.Keyboard.Vk;
using VkNet.Model.Keyboard;

namespace AsapNotificationSystem.Bot.Vk
{
    public class VkMultiBot : IReceiverBot<string[]>, ISenderBot<string>, IDisposable
    {
        public int ServiceId { get; } = 1;

        public IBotConfig ReceiverBotConfig { get; }
        public IBotConfig SenderBotConfig { get; }

        public event EventHandler<string[]> NewMessage;
        public event EventHandler<string> StartDialog;
        public event EventHandler<string> EndDialog;

        private VkApi api;
        private Random rnd;

        private Task onlineStatusTask;
        private Task messageCheckTask;

        private string longPollUrl;
        private string longPollKey;
        private string longPollTs;

        public VkMultiBot(VkBotConfig config)
        {
            ReceiverBotConfig = config;
            SenderBotConfig = config;
            rnd = new Random();

            Init();
        }

        public void Init()
        {
            api = new VkApi();
            api.Authorize(new ApiAuthParams
            {
                AccessToken = ReceiverBotConfig.Token
            });

            //if (onlineStatusTask == null || onlineStatusTask.Status != TaskStatus.Running)
            //{
            //    onlineStatusTask = new Task(OnlineStatus);
            //    onlineStatusTask.Start();
            //}

            if (messageCheckTask == null || messageCheckTask.Status != TaskStatus.Running)
            {
                messageCheckTask = new Task(CheckMessage);
                messageCheckTask.Start();
            }
        }

        public void Dispose()
        {
            onlineStatusTask?.Dispose();
            messageCheckTask?.Dispose();
            api?.Dispose();
        }

        public async void OnlineStatus()
        {
            while(true)
            {
                try
                {
                    api.Groups.EnableOnline(((VkBotConfig) ReceiverBotConfig).GroupId);
                }
                catch
                {
                    Init();
                    api.Groups.EnableOnline(((VkBotConfig) ReceiverBotConfig).GroupId);
                }

                await Task.Delay(1000 * 60 * 10);
            }
        }

        public void SendMessage(string message, string userId, IKeyboard keyboard = null)
        {
            var temp = new MessagesSendParams
            {
                RandomId = rnd.Next(),
                UserId = long.Parse(userId),
                Message = message
            };

            var tmp = ParseKeyboard(keyboard);
            if (tmp != null)
            {
                temp.Keyboard = tmp;
            }

            api.Messages.Send(temp);
        }

        public void SendMassMessage(string message, IEnumerable<string> userIds)
        {
            var temp = userIds.Select(long.Parse);
            while (temp.Any())
            {
                api.Messages.SendToUserIds(new MessagesSendParams
                {
                    RandomId = rnd.Next(),
                    UserIds = temp.Take(100),
                    Message = message
                });
                temp = temp.Skip(100);
            }
        }

        private void LoadLongPollSettings()
        {
            var temp = api.Groups.GetLongPollServer(((VkBotConfig) ReceiverBotConfig).GroupId);

            longPollUrl = temp.Server;
            longPollKey = temp.Key;
            longPollTs  = temp.Ts;
        }

        private async void CheckMessage()
        {
            while(true)
            {
                try
                {
                    var poll = api.Groups.GetBotsLongPollHistory(new BotsLongPollHistoryParams
                    {
                        Key = longPollKey,
                        Server = longPollUrl,
                        Ts = longPollTs,
                        Wait = ((VkBotConfig)ReceiverBotConfig).WaitTime
                    });

                    if (poll != null)
                    {
                        longPollTs = poll.Ts ?? longPollTs;
                        if (poll.Updates != null)
                        {
                            foreach (var update in poll.Updates)
                            {
                                if (update.Type == GroupUpdateType.MessageNew)
                                {
                                    var data = new []
                                    {
                                        update.MessageNew.Message.Text,
                                        update.MessageNew.Message.FromId?.ToString(),
                                        update.MessageNew.Message.Payload
                                    };

                                    NewMessage?.Invoke(null, data);
                                }
                                else if (update.Type == GroupUpdateType.MessageAllow)
                                {
                                    StartDialog.Invoke(null, update.MessageAllow.UserId?.ToString());
                                }
                                else if (update.Type == GroupUpdateType.MessageDeny)
                                {
                                    EndDialog.Invoke(null, update.MessageDeny.UserId?.ToString());
                                }
                            }
                        }
                    }
                }
                catch
                {
                    LoadLongPollSettings();
                    continue;
                }

                if (((VkBotConfig) ReceiverBotConfig).PauseTime > 0)
                {
                    await Task.Delay(((VkBotConfig) ReceiverBotConfig).PauseTime);
                }
            }
        }

        private MessageKeyboard ParseKeyboard(IKeyboard keyboard)
        {
            if (keyboard != null)
            {
                KeyboardBuilder kBuilder;
                if (keyboard is VkKeyboard vkKeyboard)
                {
                    kBuilder = new KeyboardBuilder(vkKeyboard.Type, true);
                    int i = 0;
                    foreach (VkButton button in vkKeyboard.Buttons)
                    {
                        kBuilder.AddButton(
                            button.Text,
                            button.PayloadText,
                            button.Color == "positive" ? KeyboardButtonColor.Positive :
                            button.Color == "negative" ? KeyboardButtonColor.Negative :
                            button.Color == "primary" ? KeyboardButtonColor.Primary :
                            KeyboardButtonColor.Default);
                        i++;
                        if (i % 4 == 0 && i != 0)
                            kBuilder.AddLine();
                        
                    }
                }
                else
                {
                    kBuilder = new KeyboardBuilder();
                    int i = 0;
                    foreach (var button in keyboard.Buttons)
                    {
                        kBuilder.AddButton(button.Text, "def");
                        i++;
                        if (i % 4 == 0 && i != 0)
                            kBuilder.AddLine();
                    }
                }
                return kBuilder.Build();
            }

            return null;
        }
    }
}
