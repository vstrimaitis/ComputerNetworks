using System;

namespace Mail.Exceptions
{
    public class SmtpException : Exception
    {
        public SmtpException() : base()
        { }

        public SmtpException(string message) : base(message)
        { }

        public SmtpException(string message, Exception innerException) : base(message, innerException)
        { }

        public SmtpException(string message, Response resp) : base(FormMessage(message, resp))
        { }

        public SmtpException(string message, Response resp, Exception innerException) : base(FormMessage(message, resp), innerException)
        { }

        private static string FormMessage(string message, Response resp)
        {
            return string.Format("{0}{1}{1}Server response:{1}{2}",
                                    message,
                                    Environment.NewLine,
                                    string.Join(Environment.NewLine, resp.ShortMessage));
        }
    }
}
