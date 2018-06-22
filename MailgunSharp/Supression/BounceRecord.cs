using System;

namespace MailgunSharp.Supression
{
  public class BounceRecord : IBounceRecord
  {
    private string address;
    public string Address
    {
      get
      {
        return address;
      }
    }

    private SmtpErrorCode code;
    public SmtpErrorCode Code
    {
      get
      {
        return code;
      }
    }

    private string error;
    public string Error
    {
      get
      {
        return error;
      }
    }

    private DateTime? createdAt;
    public DateTime? CreatedAt
    {
      get
      {
        return createdAt;
      }
    }

    public BounceRecord(string address, SmtpErrorCode statusCode = SmtpErrorCode.MAILBOX_UNAVAILABLE, string errorDescription = "", DateTime? createdAt = null)
    {
      if (checkStringIfNullOrEmpty(address))
      {
        throw new ArgumentNullException("Email Address cannot be null or empty!");
      }

      this.address = address;
      this.code = statusCode;
      this.error = errorDescription;
      this.createdAt = createdAt;
    }

    private bool checkStringIfNullOrEmpty(string str)
    {
      return (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
    }
  }
}
