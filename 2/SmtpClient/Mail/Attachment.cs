using System;
using System.IO;

namespace Mail
{
    /*
     * https://msdn.microsoft.com/en-us/library/ms526560(v=exchg.10).aspx
     */
    public class Attachment
    {
        private byte[] _content;
        public string FileName { get; private set; }

        public Attachment(string filename)
        {
            FileName = filename;
            _content = File.ReadAllBytes(filename);
        }

        public string[] ToMimeString()
        {
            return new string[]
            {
                "Content-Type: " + MimeHelper.GetMimeType(FileName) + "; name=\""+FileName+"\"",
                "Content-Disposition: attachment; filename=\"" + FileName + "\"",
                "Content-Transfer-Encoding: base64",
                "",
                Convert.ToBase64String(_content)
            };
        }
    }
}
