﻿using System;
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
                SendCommandWithResponse(SmtpCommands.AuthenticateLogin);
                SendCommandWithResponse(_credentials.Login);
                SendCommandWithResponse(_credentials.Password);
                /*Debug.WriteLine(SendCommandWithResponse(SmtpCommands.AuthenticatePlain));
                Debug.WriteLine(SendCommandWithResponse(_credentials.PlainLogin));*/
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
            
            Debug.WriteLine("<< "+_responseManager.GetResponse());
            SendCommandWithResponse(string.Format("{0} {1}", SmtpCommands.HelloExtended, Dns.GetHostName()));
        }

        public void Send(MailMessage message)
        {
            SendCommandWithResponse(string.Format("{0} <{1}>", SmtpCommands.MailFrom, message.From.Address));
            foreach(var r in message.To)
            {
                SendCommandWithResponse(string.Format("{0} <{1}>", SmtpCommands.Recipient, r.Address));
            }
            SendCommandWithResponse(SmtpCommands.MailData);
            foreach(var l in message.ToMimeString())
            {
                SendCommand(l);
            }

            Debug.WriteLine("<< "+_responseManager.GetResponse());
        }

        public void Dispose()
        {
            SendCommandWithResponse(SmtpCommands.Quit);
            _reader.Dispose();
            _writer.Dispose();
            _stream.Dispose();
            _client.Close();
        }

        private void SendCommand(string command)
        {
            Debug.WriteLine(string.Format(">> {0}", command));
            _writer.WriteLine(command);
            _writer.Flush();
        }

        private Response SendCommandWithResponse(string command)
        {
            SendCommand(command);
            var resp = _responseManager.GetResponse();
            foreach (var l in resp.Message)
                Debug.WriteLine(string.Format("<< {0}", l));
            return resp;
        }
    }
}
