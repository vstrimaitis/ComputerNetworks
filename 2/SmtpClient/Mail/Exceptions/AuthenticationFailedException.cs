namespace Mail.Exceptions
{
    public class AuthenticationFailedException : SmtpException
    {
        public AuthenticationFailedException(Response r) : base("Failed to authenticate with the mail server.", r)
        { }
    }
}
