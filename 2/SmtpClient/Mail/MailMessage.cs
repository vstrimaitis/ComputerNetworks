using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace Mail
{
    /*
     * https://msdn.microsoft.com/en-us/library/ms526560(v=exchg.10).aspx
     */
    public class MailMessage
    {
        private readonly string _boundary;

        public IEnumerable<MailAddress> To { get; private set; }
        public MailAddress From { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }
        public IEnumerable<Attachment> Attachments { get; private set; }

        public MailMessage(string from, string to, string subject, string body, IEnumerable<string> attachmentNames = null)
            : this(from, new List<string>() { to }, subject, body, attachmentNames)
        { }

        public MailMessage(string from, IEnumerable<string> to, string subject, string body, IEnumerable<string> attachmentNames = null)
        {
            From = new MailAddress(from);
            To = to.Select(x => new MailAddress(x));
            Subject = subject;
            // dot stuffing
            Body = string.Join(Environment.NewLine, body.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                                                        .Select(x => x.Length > 0 && x[0] == '.' ? x = "." + x : x));
            Attachments = attachmentNames == null ? Enumerable.Empty<Attachment>() : attachmentNames.Select(x => new Attachment(x));
            _boundary = MimeHelper.GenerateBoundary();
        }

        public string[] ToMimeString()
        {
            // TODO
            var lines = new List<string>()
            {
                "MIME-Version: 1.0",
                "Content-Type: multipart/mixed; boundary=\"" + _boundary +"\"",
                "From: \"" + From.DisplayName + "\" <" + From.Address + ">",
                "To: " + string.Join(", ", To.Select(x => "\"" + x.DisplayName + "\" <"+x.Address+">")),
                "Subject: " + Subject,
                "Date: " + DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + " +0000",
                "--"+_boundary,
                Body,
            };
            foreach(var a in Attachments)
            {
                lines.AddRange(new string[] { "", "", "--" + _boundary });
                lines.AddRange(a.ToMimeString());
            }
            lines.AddRange(new string[] { "--" + _boundary + "--", "", "." });
            return lines.ToArray();
        }
    }
}
