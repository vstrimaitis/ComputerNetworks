using Logging;
using Mail;
using Mail.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmtpClientUI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly List<MailServer> Servers = new List<MailServer>()
        {
            new MailServer("smtp.gmail.com", 465), // vstrimaitis.cn@gmail.com, ComputerNetwork
            new MailServer("smtp.mail.yahoo.com", 465), // vstrimaitis.cn@yahoo.com, ComputerNetworkYahoo
            new MailServer("smtp.mail.com", 587), // vstrimaitis.cn@mail.com, ComputerNetwork
        };
        private SmtpClient _client;
        private const string LogFile = "log.txt";

        public LoginWindow()
        {
            InitializeComponent();
            serverBox.ItemsSource = Servers;
            serverBox.SelectedIndex = 0;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            string email = emailBox.Text;
            string password = passwordBox.Password;
            if(string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("You must fill in both fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                var server = serverBox.SelectedItem as MailServer;
                if (server == null)
                    server = new MailServer(serverBox.Text, int.Parse(portBox.Text), domainBox.Text);
                _client = new SmtpClient(new FileLogger(LogFile, true));
                _client.Connect(server);
                _client.Credentials = new Credentials(email, password, Encoding.UTF8);
                var mailWindow = new MainWindow(_client);
                mailWindow.Show();
                this.Close();
            }
            catch(SmtpException) // Add login failed exception
            {
                _client?.Dispose();
                MessageBox.Show("Login failed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch(FormatException)
            {
                _client?.Dispose();
                MessageBox.Show("Invalid email format!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch(Exception ex)
            {
                _client?.Dispose();
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void serverBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = serverBox.SelectedItem as MailServer;
            HandleServerInput(selected);
        }

        private void serverBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var selected = serverBox.SelectedItem as MailServer;
            HandleServerInput(selected);
        }

        private void HandleServerInput(MailServer ms)
        {
            if (ms == null)
            {
                portBox.IsEnabled = true;
                portBox.Text = "";
                domainBox.IsEnabled = true;
                domainBox.Text = "";
            }
            else
            {
                portBox.IsEnabled = false;
                portBox.Text = ms.Port.ToString();
                domainBox.IsEnabled = false;
                domainBox.Text = ms.EmailDomain;
            }
        }
    }
}
