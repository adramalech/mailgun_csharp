using System;

namespace MailgunSharp.Wrappers
{
  public interface IDateTimeProvider
  {
    DateTime Now { get; }
    DateTime UtcNow { get; }
  }
}