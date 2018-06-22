using System;

namespace MailgunSharp.Supression
{
  public interface IBounceRecord
  {
    string Address { get; }
    SmtpErrorCode Code { get; }
    string Error { get; }
    DateTime? CreatedAt { get; }
  }
}
