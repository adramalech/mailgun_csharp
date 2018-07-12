using System;

namespace MailgunSharp.Messages
{
  public sealed class FileAttachment : IFileAttachment
  {
    private readonly string filename;

    /// <summary>
    /// The name of the file to be attached to a message.
    /// </summary>
    /// <value>string</value>
    public string FileName
    {
      get
      {
        return filename;
      }
    }

    private readonly byte[] data;

    /// <summary>
    /// The contents of a file represented as a byte array.
    /// </summary>
    /// <value>byte[]</value>
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
      if (checkStringIfNullEmptyWhitespace(filename))
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

    /// <summary>
    /// Check if the string only is null, empty, or whitespace.
    /// </summary>
    /// <param name="str">The string to check.</param>
    /// <returns>True, if the string is only null, empty, or whitespace; false, if it isn't null, empty, or whitespace.</returns>
    private bool checkStringIfNullEmptyWhitespace(string str)
    {
      return (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
    }
  }
}
