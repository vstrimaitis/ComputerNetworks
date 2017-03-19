using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
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
    /// Interaction logic for DetailedMessageBox.xaml
    /// </summary>
    public partial class DetailedMessageBox : Window
    {
        public static void Show(Window parent, string message, string header, string details, MessageBoxImage icon)
        {
            Icon image = null;
            SystemSound sound = null;
            switch (icon)
            {
                case MessageBoxImage.Asterisk:
                    image = SystemIcons.Asterisk;
                    sound = SystemSounds.Asterisk;
                    break;
                case MessageBoxImage.Error:
                    image = SystemIcons.Error;
                    sound = SystemSounds.Beep;
                    break;
                case MessageBoxImage.Exclamation:
                    image = SystemIcons.Exclamation;
                    sound = SystemSounds.Exclamation;
                    break;
                case MessageBoxImage.Question:
                    image = SystemIcons.Question;
                    sound = SystemSounds.Question;
                    break;
            }
            var window = new DetailedMessageBox(parent, message, header, details, image.ToBitmap(), sound);
            window.ShowDialog();
        }

        private DetailedMessageBox(Window parent, string message, string header, string details, Bitmap bitmap, SystemSound sound)
        {
            this.Owner = parent;
            InitializeComponent();
            messageLabel.Text = message;
            Title = header;
            detailsBox.Text = details;
            sound?.Play();
            if (bitmap == null)
                return;
            var hBitmap = bitmap.GetHbitmap();
            icon.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            Height -= detailsBox.Height;
            okButton.Margin = new Thickness(okButton.Margin.Left, okButton.Margin.Top - detailsBox.Height, okButton.Margin.Right, okButton.Margin.Bottom);
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Height += detailsBox.Height;
            okButton.Margin = new Thickness(okButton.Margin.Left, okButton.Margin.Top + detailsBox.Height, okButton.Margin.Right, okButton.Margin.Bottom);
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
