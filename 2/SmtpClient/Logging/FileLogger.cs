using System;
using System.Globalization;
using System.IO;

namespace Logging
{
    public class FileLogger : ILogger
    {
        private StreamWriter _writer;
        public bool AutoFlush { get; set; }

        public FileLogger(string path, bool autoFlush = false)
        {
            _writer = new StreamWriter(new FileStream(path, FileMode.Append));
            AutoFlush = autoFlush;
        }

        public void Write(object message)
        {
            Write(message.ToString());
        }

        public void Write(string message)
        {
            _writer.Write(string.Format("[{0}]\t{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), message));
            if (AutoFlush)
                Flush();
        }

        public void Write(string format, params object[] parameters)
        {
            Write(string.Format(format, parameters));
        }

        public void WriteLine(object message)
        {
            WriteLine(message.ToString());
        }

        public void WriteLine(string message)
        {
            _writer.WriteLine(string.Format("[{0}]\t{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), message));
            if (AutoFlush)
                Flush();
        }

        public void WriteLine(string format, params object[] parameters)
        {
            WriteLine(string.Format(format, parameters));
        }

        public void Flush()
        {
            _writer.Flush();
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
