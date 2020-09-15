using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsapNotificationSystem.Bot.Keyboard.Vk
{
    public class VkKeyboard : IKeyboard
    {
        public List<IButton> Buttons { get; private set; }
        public string Type { get; private set; }

        public VkKeyboard(string type = null)
        {
            Buttons = new List<IButton>();
            Type = type;
        }

        public VkKeyboard(IEnumerable<VkButton> vkButtons, string type = null)
        {
            if (vkButtons == null)
                Buttons = new List<IButton>();
            else
                Buttons = vkButtons.Select(x => (IButton)x).ToList();

            Type = type;
        }

        public VkKeyboard(string type, params VkButton[] vkButtons)
        {
            Buttons = vkButtons == null ? new List<IButton>() : vkButtons.Select(x => (IButton)x).ToList();
            Type = type;
        }

        public IKeyboard AddButton(VkButton button)
        {
            ((List<IButton>)Buttons).Add(button);
            return this;
        }

        public IKeyboard AddButtons(IEnumerable<VkButton> buttons)
        {
            ((List<IButton>)Buttons).AddRange(buttons);
            return this;
        }

        public VkKeyboard Clone()
        {
            return new VkKeyboard(Buttons.Select(x => ((VkButton) x).Clone()), Type);
        }
    }
}
