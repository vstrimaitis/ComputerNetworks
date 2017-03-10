using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail
{
    public class ResponseManager
    {
        private StreamReader _reader;
        public ResponseManager(StreamReader reader)
        {
            _reader = reader;
        }
        public Response GetResponse()
        {
            List<string> msg = new List<string>();
            do
            {
                msg.Add(_reader.ReadLine());
            } while (msg[msg.Count - 1].Length > 3 && msg[msg.Count - 1][3] != ' ');
            int code = int.Parse(msg[msg.Count - 1].Substring(0, 3));
            return new Response(code, msg);
        }
    }
}
