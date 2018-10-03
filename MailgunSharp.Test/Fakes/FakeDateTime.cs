using System;
using MailgunSharp.Wrappers;

namespace MailgunSharp.Test.Fakes
{
  public class FakeDateTime : IDateTimeProvider
  {
    private DateTimeKind? kind;

    public FakeDateTime()
    {
      this.kind = null;
    }

    public FakeDateTime(DateTimeKind kind)
    {
      this.kind = kind;
    }

    public DateTime Now => (this.kind.HasValue) ? new DateTime(1970, 1, 1, 0, 0, 0, this.kind.Value) : new DateTime(1970, 1, 1, 0, 0, 0);

    public DateTime UtcNow => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
  }
}