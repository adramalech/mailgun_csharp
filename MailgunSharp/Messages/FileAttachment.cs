namespace MailgunSharp.Messages
{
  public sealed class FileAttachment : IFileAttachment
  {
    public string FileName { get; set; }
    public byte[] Data { get; set; }
  }
}
