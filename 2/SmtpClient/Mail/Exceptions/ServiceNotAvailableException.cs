using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail.Exceptions
{
    public class ServiceNotAvailableException : Exception
    {
        public ServiceNotAvailableException() : base()
        { }

        public ServiceNotAvailableException(string message) : base(message)
        { }

        public ServiceNotAvailableException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
