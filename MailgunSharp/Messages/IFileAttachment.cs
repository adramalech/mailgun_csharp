using System.IO;

namespace MailgunSharp.Messages
{
  public interface IFileAttachment
  {
    string FileName { get; set; }
    byte[] Data { get; set; }
  }
}
