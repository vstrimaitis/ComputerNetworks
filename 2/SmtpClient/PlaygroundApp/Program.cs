using Mail;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string server = "smtp.gmail.com";
            int port = 465;
            /*string username = "dnN0cmltYWl0aXNAZ21haWwuY29t";
            string pass = "enJ3eWdic2NscWpueXh0ag==";
            using (var client = new TcpClient(server, port))
            {
                using (var stream = new SslStream(client.GetStream()))
                {
                    stream.AuthenticateAsClient(server);
                    var reader = new StreamReader(stream);
                    var writer = new StreamWriter(stream);
                    writer.AutoFlush = true;

                    Console.WriteLine(reader.ReadLine());
                    writer.Write(string.Format("HELO {0}\r\n", Dns.GetHostName()));
                    Console.WriteLine(reader.ReadLine());
                    writer.Write("AUTH LOGIN\r\n");
                    Console.WriteLine(reader.ReadLine());
                    writer.Write(string.Format("{0}\r\n", username));
                    Console.WriteLine(reader.ReadLine());
                    writer.Write(string.Format("{0}\r\n", pass));
                    Console.WriteLine(reader.ReadLine());

                }
            }*/
            using (var client = new SmtpClient(server, port))
            {
                client.Credentials = new Credentials("vstrimaitis@gmail.com", "zrwygbsclqjnyxtj", System.Text.Encoding.UTF8);
                Console.WriteLine(client.Credentials.Login);
                Console.WriteLine(client.Credentials.Password);
            }
        }
    }
}
