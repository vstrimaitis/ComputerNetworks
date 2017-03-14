using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mail
{
    public struct Response
    {
        public int Code { get; private set; }
        public IEnumerable<string> Message { get; private set; }
        public IEnumerable<string> ShortMessage { get; private set; } // The original message except with no reply or status code

        public Response(int code, IEnumerable<string> message)
        {
            Code = code;
            Message = message;
            ShortMessage = message.Select(x => x.Substring(4, x.Length - 4))
                                  .Select(x => Regex.IsMatch(x, @"\d+\.\d+\.\d+.*") ? string.Join("", Regex.Split(x, @"\d+\.\d+\.\d+").Skip(1)).Trim()
                                                                                    : x);
        }

        public override string ToString()
        {
            return string.Join("\n", Message);
        }
    }
}
