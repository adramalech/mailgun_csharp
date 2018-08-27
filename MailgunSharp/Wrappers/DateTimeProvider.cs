using System;

namespace MailgunSharp.Wrappers
{
  public sealed class DateTimeProvider : IDateTimeProvider
  {
    public DateTime Now => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;
  }
}