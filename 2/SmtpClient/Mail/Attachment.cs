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
        public string FilePath { get; private set; }
        public string FileName { get; private set; }

        public Attachment(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            _content = File.ReadAllBytes(filePath);
        }

        public string[] ToMimeString()
        {
            return new string[]
            {
                "Content-Type: " + MimeHelper.GetMimeType(FilePath) + "; name=\""+FileName+"\"",
                "Content-Disposition: attachment; filename=\"" + FileName + "\"",
                "Content-Transfer-Encoding: base64",
                "",
                Convert.ToBase64String(_content)
            };
        }
    }
}
