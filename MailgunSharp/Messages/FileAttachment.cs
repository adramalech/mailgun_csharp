using System;

namespace MailgunSharp.Messages
{
  public sealed class FileAttachment : IFileAttachment
  {
    /// <summary>
    /// The name of the file to be attached to a message.
    /// </summary>
    /// <value>string</value>
    private readonly string filename;
    public string FileName
    {
      get
      {
        return filename;
      }
    }

    /// <summary>
    /// The contents of a file represented as a byte array.
    /// </summary>
    /// <value>byte[]</value>
    private readonly byte[] data;
    public byte[] Data
    {
      get
      {
        return data;
      }
    }

    /// <summary>
    /// Create an instance of file attachment.
    /// </summary>
    /// <param name="filename">The name of the file to be attached.</param>
    /// <param name="data">The file content of the file to be attached.</param>
    public FileAttachment(string filename, byte[] data)
    {
      if (checkStringIfNullOrEmpty(filename))
      {
        throw new ArgumentNullException("Filename cannot be null or empty!");
      }

      if (data == null || data.Length < 1)
      {
        throw new ArgumentNullException("File data be null or empty!");
      }

      this.filename = filename;
      this.data = data;
    }

    private bool checkStringIfNullOrEmpty(string str)
    {
      return (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
    }
  }
}
