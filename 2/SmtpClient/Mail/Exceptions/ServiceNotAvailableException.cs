namespace Mail.Exceptions
{
    public class ServiceNotAvailableException : SmtpException
    {
        public ServiceNotAvailableException(Response r) : base("The service is not available.", r)
        { }
    }
}
