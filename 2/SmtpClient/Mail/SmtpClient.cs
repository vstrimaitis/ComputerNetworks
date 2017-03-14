using Mail.Exceptions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace Mail
{
    /*
     * Return codes: http://www.greenend.org.uk/rjk/tech/smtpreplies.html#TURN
     * http://www.serversmtp.com/en/smtp-error
     * Status codes: https://www.usps.org/info/smtp_status.html
     * More status codes: https://www.iana.org/assignments/smtp-enhanced-status-codes/smtp-enhanced-status-codes.xml
     */
    public class SmtpClient : IDisposable
    {
        private TcpClient _client;
        private SslStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private Credentials _credentials;
        private ResponseManager _responseManager;

        public string Host { get; private set; }
        public int Port { get; private set; }

        public Credentials Credentials
        {
            get
            {
                return _credentials;
            }
            set
            {
                _credentials = value;
                SendCommand(SmtpCommands.Authenticate, 334);
                SendCommand(_credentials.Login, 334);
                SendCommand(_credentials.Password, 235);
            }
        }

        public SmtpClient(string host, int port)
        {
            Host = host;
            Port = port;
            _client = new TcpClient(host, port);
            _stream = new SslStream(_client.GetStream());
            _stream.AuthenticateAsClient(host);
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream);

            _responseManager = new ResponseManager(_reader);
            
            Debug.WriteLine("S:\t"+GetResponse());
            SendCommand(string.Format("{0} {1}", SmtpCommands.Hello, Dns.GetHostName()), 250);
        }
        
        public void Send(MailMessage message)
        {
            SendCommand(string.Format("{0} <{1}>", SmtpCommands.MailFrom, message.From.Address), 250);
            foreach(var r in message.To)
            {
                SendCommand(string.Format("{0} <{1}>", SmtpCommands.Recipient, r.Address), 250);
            }
            SendCommand(SmtpCommands.MailData, 354);
            foreach(var l in message.ToMimeString())
            {
                SendCommand(l);
            }

            Debug.WriteLine("S:\t" + GetResponse(250));
        }

        public void Dispose()
        {
            SendCommand(SmtpCommands.Quit, 221);
            _reader.Dispose();
            _writer.Dispose();
            _stream.Dispose();
            _client.Close();
        }

        private void SendCommand(string command, int? expectedResponseCode = null)
        {
            if(command.Length < 500)
                Debug.WriteLine(string.Format("C:\t{0}", command));
            _writer.WriteLine(command);
            _writer.Flush();
            if (!expectedResponseCode.HasValue)
                return;
            var resp = GetResponse(expectedResponseCode);
            foreach (var l in resp.Message)
                Debug.WriteLine(string.Format("S:\t{0}", l));
        }

        private Response GetResponse(int? expectedCode = null)
        {
            var r = _responseManager.GetResponse();
            if (expectedCode.HasValue && r.Code != expectedCode.Value)
            {
                Debug.WriteLine(string.Format("!!\tExpected {0}, got {1}", expectedCode.Value, r.Code));
                throw new ServiceNotAvailableException(r); // TODO
            }
            return r;
        }
    }
}
