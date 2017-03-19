using System;

namespace Mail.Exceptions
{
    public class SmtpException : Exception
    {
        public string FullResponse { get; private set; }

        public SmtpException() : base()
        { }

        public SmtpException(string message) : base(message)
        { }

        public SmtpException(string message, Exception innerException) : base(message, innerException)
        { }

        public SmtpException(string message, Response resp) : this(message, resp, null)
        { }

        public SmtpException(string message, Response resp, Exception innerException) : base(message, innerException)
        {
            FullResponse = string.Join(" ", resp.ShortMessage);
        }
    }
}
