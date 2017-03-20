using Logging;
using Mail.Exceptions;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace Mail
{
    public class SmtpClient : IDisposable
    {
        private TcpClient _client;
        private Stream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private Credentials _credentials;
        private ResponseManager _responseManager;
        private ILogger _logger;
        
        public MailServer Host { get; private set; }

        public Credentials Credentials
        {
            get
            {
                return _credentials;
            }
            set
            {
                _credentials = value;
                if (!_credentials.LoginPlain.EndsWith(Host.EmailDomain))
                    _credentials.LoginPlain += "@" + Host.EmailDomain;
                SendCommand(SmtpCommands.Authenticate, 334);
                SendCommand(_credentials.Login, 334);
                SendCommand(_credentials.Password, 235);
            }
        }

        public SmtpClient(ILogger logger = null)
        {
            _logger = logger;
        }

        public void Connect(MailServer host)
        {
            Host = host;
            _client = new TcpClient(Host.Server, Host.Port);
            try
            {
                _stream = new SslStream(_client.GetStream());
                ((SslStream)_stream).AuthenticateAsClient(Host.Server);
            }
            catch(Exception)
            {
                _client.Close();
                _client = new TcpClient(Host.Server, Host.Port);
                _stream = _client.GetStream();
            }
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream);

            _responseManager = new ResponseManager(_reader);

            _logger?.WriteLine("S:\t" + GetResponse());
            SendCommand(string.Format("{0} {1}", SmtpCommands.HelloExtended, Dns.GetHostName()), 250);
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

            _logger?.WriteLine("S:\t" + GetResponse(250));
        }

        public void Dispose()
        {
            try
            {
                SendCommand(SmtpCommands.Quit, 221);
            }
            catch (Exception)
            { }
            _logger?.Dispose();
            _reader?.Dispose();
            _writer?.Dispose();
            _stream?.Dispose();
            _client?.Close();
        }

        private void SendCommand(string command, int? expectedResponseCode = null)
        {
            _logger?.WriteLine(string.Format("C:\t{0}", command));
            _writer.WriteLine(command);
            _writer.Flush();
            if (!expectedResponseCode.HasValue)
                return;
            var resp = GetResponse(expectedResponseCode);
            foreach (var l in resp.Message)
                _logger?.WriteLine(string.Format("S:\t{0}", l));
        }

        private Response GetResponse(int? expectedCode = null)
        {
            var r = _responseManager.GetResponse();
            if (expectedCode.HasValue && r.Code != expectedCode.Value)
            {
                _logger?.WriteLine(string.Format("!!\tExpected {0}, got {1}", expectedCode.Value, r.Code));
                if (r.Code == 421)
                    throw new ServiceNotAvailableException(r);
                else if (r.Code == 535)
                    throw new AuthenticationFailedException(r);
                else if (r.Code == 552 || r.Code == 554 || r.Code == 451 || r.Code == 452)
                    throw new MailActionAbortedException(r);
                else if(r.Code / 100 == 4 || r.Code / 100 == 5)
                    throw new SmtpException("Something unexpected happened.", r);
            }
            return r;
        }
    }
}
