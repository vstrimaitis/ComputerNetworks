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
            using (var client = new SmtpClient(server, port))
            {
                client.Credentials = new Credentials("vstrimaitis@gmail.com", "zrwygbsclqjnyxtj", System.Text.Encoding.UTF8);
                Console.WriteLine(client.Credentials.Login);
                Console.WriteLine(client.Credentials.Password);

                client.Send(new MailMessage("vstrimaitis@gmail.com",
                                            "vstrimaitis@gmail.com",
                                            "Test email",
                                            @"This is a test email

It is nothing special at all

Just a silly email
thats it










.




;)"));
            }
        }
    }
}
