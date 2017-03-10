using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail
{
    static class SmtpCommands
    {
        public const string Hello = "HELO";
        public const string MailFrom = "MAIL FROM:";
        public const string Recipient = "RCPT TO:";
        public const string MailData = "DATA";
        public const string Reset = "RSET";
        public const string Verify = "VRFY";
        public const string NoOperation = "NOOP";
        public const string Quit = "QUIT";

        public const string HelloExtended = "EHLO";
        public const string AuthenticatePlain = "AUTH PLAIN";
        public const string AuthenticateLogin = "AUTH LOGIN";
        public const string AuthenticateCramMd5 = "AUTH CRAM-MD5";
        public const string StartTls = "STARTTLS";
        public const string Size = "SIZE";
        public const string Help = "HELP";
    }
}
