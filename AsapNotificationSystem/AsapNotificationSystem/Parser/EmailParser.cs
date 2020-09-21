using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MimeKit;

namespace AsapNotificationSystem.Parser
{
    public class EmailParser : IParser<MimeMessage, bool>
    {
        private IEnumerable<string> from;
        private IEnumerable<string> themes;
        private IEnumerable<string> attachmentNames;

        private bool needAttachment;

        public EmailParser(IEnumerable<string> from = null, IEnumerable<string> themes = null, bool needAttachment = false, IEnumerable<string> attachmentNames = null)
        {
            this.from = from;
            this.themes = themes;
            this.needAttachment = needAttachment;
            this.attachmentNames = needAttachment ? attachmentNames : null;
        }

        public bool Parse(MimeMessage message)
        {
            try
            {
                if (from != null && message.From.All(x => !from.Contains(x.ToString().Split('<')[1].Replace(">", ""))))
                    return false;

                if (themes != null && !themes.Contains(message.Subject))
                    return false;

                if (needAttachment && (message.Attachments == null || !message.Attachments.Any()))
                    return false;

                if (attachmentNames != null &&
                    message.Attachments.Any(x => !attachmentNames.Contains(x.ContentDisposition.FileName.ToLower())))
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
