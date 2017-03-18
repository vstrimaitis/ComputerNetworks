using System;
using System.Text;

namespace Mail
{
    public class Credentials
    {
        private Encoding _encoding;
        private string _loginPlain;
        public string Login { get; private set; }
        public string LoginPlain
        {
            get
            {
                return _loginPlain;
            }
            set
            {
                _loginPlain = value;
                Login = Convert.ToBase64String(_encoding.GetBytes(value));
            }
        }
        public string Password { get; private set; }

        public Credentials(string login, string password, Encoding encoding)
        {
            _encoding = encoding;
            LoginPlain = login;
            var bytes = encoding.GetBytes(password);
            Password = Convert.ToBase64String(bytes);
        }
    }
}
