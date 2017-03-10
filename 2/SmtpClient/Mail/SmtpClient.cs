using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace Mail
{
    /*
     * Return codes: http://www.greenend.org.uk/rjk/tech/smtpreplies.html#TURN
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
                Debug.WriteLine(SendCommand("AUTH LOGIN"));
                Debug.WriteLine(SendCommand(_credentials.Login));
                Debug.WriteLine(SendCommand(_credentials.Password));
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
            _writer.NewLine = "\r\n";

            _responseManager = new ResponseManager(_reader);

            var response = _responseManager.GetResponse();
            Debug.WriteLine(response);
            Debug.WriteLine(SendCommand(string.Format("{0} {1}", SmtpCommands.HelloExtended, Dns.GetHostName())));
            
        }

        private Response SendCommand(string command)
        {
            _writer.WriteLine(command);
            _writer.Flush();
            return _responseManager.GetResponse();
        }

        public void Dispose()
        {
            Debug.WriteLine(SendCommand(SmtpCommands.Quit));
            _reader.Dispose();
            _writer.Dispose();
            _stream.Dispose();
            _client.Close();
        }
    }
}
