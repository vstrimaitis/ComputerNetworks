namespace Mail.Exceptions
{
    public class MailActionAbortedException : SmtpException
    {
        public MailActionAbortedException(Response r) : base("The requested action was aborted.", r)
        { }
    }
}
