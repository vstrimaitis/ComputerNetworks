using System.Collections.Generic;

namespace Mail
{
    public struct Response
    {
        public int Code { get; private set; }
        public IEnumerable<string> Message { get; private set; }

        public Response(int code, IEnumerable<string> message)
        {
            Code = code;
            Message = message;
        }

        public override string ToString()
        {
            return string.Join("\n", Message);
        }
    }
}
