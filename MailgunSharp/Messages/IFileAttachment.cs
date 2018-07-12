using System.IO;

namespace MailgunSharp.Messages
{
  public interface IFileAttachment
  {
    /// <summary>
    /// The name of the file to be attached to a message.
    /// </summary>
    /// <value>string</value>
    string FileName { get; }

    /// <summary>
    /// The contents of a file represented as a byte array.
    /// </summary>
    /// <value>byte[]</value>
    byte[] Data { get; }
  }
}
