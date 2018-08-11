using System;

namespace MailgunSharp.Wrappers
{
  public sealed class DateTimeProvider : IDateTimeProvider
  {
    public DateTime Now
    {
      get
      {
        return DateTime.Now;
      }
    }

    public DateTime UtcNow
    {
      get
      {
        return DateTime.UtcNow;
      }
    }
  }
}