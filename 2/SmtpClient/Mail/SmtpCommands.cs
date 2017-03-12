namespace Mail
{
    static class SmtpCommands
    {
        public const string Hello = "HELO";
        public const string Authenticate = "AUTH LOGIN";
        public const string MailFrom = "MAIL FROM:";
        public const string Recipient = "RCPT TO:";
        public const string MailData = "DATA";
        public const string Quit = "QUIT";
    }
}
