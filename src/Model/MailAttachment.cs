using System.IO;

namespace Assistance.Operational.Model
{
    public class MailAttachment
    {
        public Stream File { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
