using System;
using System.Collections.Generic;
using System.Text;

namespace AsapNotificationSystem.Bot.Keyboard.Vk
{
	public class VkButton : IButton
	{
		public string Text { get; set; }
		public string Color { get; set; }
		public string PayloadText { get; set; }

        public VkButton(string text, string color = null, string payloadText = null)
        {
            Text = text;
            Color = color;
            PayloadText = payloadText ?? "";
        }

        public VkButton Clone()
        {
            return new VkButton(Text, Color, PayloadText);
        }
	}
}
