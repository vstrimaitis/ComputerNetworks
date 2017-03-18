using System;
using System.Linq;

namespace Mail
{
    public class MailServer
    {
        public string Server { get; private set; }
        public int Port { get; private set; }
        public string EmailDomain { get; private set; }

        public MailServer(string server, int port)
            : this(server, port, "")
        {
            var parts = Server.Split('.');
            EmailDomain = string.Join(".", parts.Skip(Math.Max(0, parts.Length - 2)));
        }

        public MailServer(string server, int port, string emailDomain)
        {
            Server = server;
            Port = port;
            EmailDomain = emailDomain;
        }
    }
}
