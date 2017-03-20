using System;
using System.IO;

namespace Mail
{
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
