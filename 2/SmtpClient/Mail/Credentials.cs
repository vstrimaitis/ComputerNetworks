using System;
using System.Text;

namespace Mail
{
    public class Credentials
    {
        public string Login { get; private set; }
        public string Password { get; private set; }
        public string PlainLogin { get; private set; }

        public Credentials(string login, string password, Encoding encoding)
        {
            var bytes = encoding.GetBytes(login);
            Login = Convert.ToBase64String(bytes);
            bytes = encoding.GetBytes(password);
            Password = Convert.ToBase64String(bytes);
            bytes = encoding.GetBytes(login + "\0" + login + "\0" + password);
            PlainLogin = Convert.ToBase64String(bytes);
        }
    }
}
