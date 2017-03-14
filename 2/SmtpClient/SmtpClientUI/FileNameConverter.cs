using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace SmtpClientUI
{
    class FileNameConverter : IValueConverter
    {
        private const int MaxFileNameLength = 55;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Join(";"+Environment.NewLine,
                ((FileInfo)value).Name
                                 .Split(';')
                                 .Select(x => Path.GetFileName(x))
                                 .Select(x => x.Length > MaxFileNameLength ? x.Substring(0, MaxFileNameLength-3) +"..." : x));

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
