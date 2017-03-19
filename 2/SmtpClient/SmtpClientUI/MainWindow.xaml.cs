using Logging;
using Mail;
using Mail.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
namespace SmtpClientUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [Flags]
        enum MissingProperty
        {
            None        = 0b000,
            Recipient   = 0b001,
            Subject     = 0b010,
            Body        = 0b100
        }

        private SmtpClient _client;

        public MainWindow(SmtpClient client)
        {
            _client = client;
            InitializeComponent();
        }
        

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            recipientTextBox.Clear();
            subjectTextBox.Clear();
            bodyTextBox.Clear();
            attachmentsList.ItemsSource = null;
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.Multiselect = true;
            if(dialog.ShowDialog() == true)
            {
                attachmentsList.ItemsSource = dialog.FileNames.Select(x => new FileInfo(x));
            }
        }
        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            var to = recipientTextBox.Text;
            var subject = subjectTextBox.Text;
            var body = bodyTextBox.Text;
            var attachments = attachmentsList.ItemsSource as IEnumerable<FileInfo>;
            var missing = MissingProperty.None;
            if (string.IsNullOrWhiteSpace(to))
                missing |= MissingProperty.Recipient;
            if (string.IsNullOrWhiteSpace(subject))
                missing |= MissingProperty.Subject;
            if (string.IsNullOrWhiteSpace(body))
                missing |= MissingProperty.Body;
            
            if(missing.HasFlag(MissingProperty.Recipient))
            {
                MessageBox.Show("The email must have a recipient!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(missing != MissingProperty.None)
            {
                var items = missing.ToString().Split(',').Select(x => x.ToLower().Trim());
                var propList = string.Join(", ", items.Take(items.Count()-1)) + (items.Count() > 1 ? " and " : "") + items.LastOrDefault();
                if(MessageBox.Show(string.Format("Are you sure you want to leave the {0} empty?", propList), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
            }

            try
            {
                _client.Send(new MailMessage(_client.Credentials.LoginPlain,
                                             Regex.Split(to, "[;, ]").Where(x => !string.IsNullOrWhiteSpace(x)),
                                             subject,
                                             body,
                                             attachments?.Select(x => x.FullName))
                                             );
                MessageBox.Show("Email sent successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.None);
            }
            catch(MailActionAbortedException ex)
            {
                DetailedMessageBox.Show(this, "The send action was aborted.", "Send error", ex.FullResponse, MessageBoxImage.Error);
                return;
            }
            catch(ServiceNotAvailableException ex)
            {
                DetailedMessageBox.Show(this, ex.Message, "Send error", ex.FullResponse, MessageBoxImage.Error);
                return;
            }
            catch(SmtpException ex)
            {
                DetailedMessageBox.Show(this, ex.Message, "Smtp error", ex.FullResponse, MessageBoxImage.Error);
                return;
            }
            catch(FormatException ex)
            {
                MessageBox.Show(ex.Message, "Format exception", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch(Exception ex)
            {
                DetailedMessageBox.Show(this, "An unexpected error occurred.", "Error", ex.Message, MessageBoxImage.Error);
                return;
            }
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _client.Dispose();
        }
    }
}
