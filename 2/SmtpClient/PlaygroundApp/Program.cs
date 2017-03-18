using Mail;
using System;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var accounts = new[]{
                new { Host = new { Server = "smtp.gmail.com", Port = 465 }, Email = "vstrimaitis.cn@gmail.com", Pass = "ComputerNetwork"},
                new { Host = new { Server = "smtp.mail.yahoo.com", Port = 465 }, Email = "vstrimaitis.cn@yahoo.com", Pass = "ComputerNetworkYahoo" },
            };
            int accIndex = 0;
            using (var client = new SmtpClient())
            {
                client.Connect(new MailServer(accounts[accIndex].Host.Server, accounts[accIndex].Host.Port));
                client.Credentials = new Credentials(accounts[accIndex].Email, accounts[accIndex].Pass, System.Text.Encoding.UTF8);

                client.Send(new MailMessage(accounts[accIndex].Email,
                                            new string[]{
                                            "vstrimaitis@gmail.com",
                                            },
                                            "Test email",
                                            @"This is a test email with an attachment"));
            }
        }
    }
}
