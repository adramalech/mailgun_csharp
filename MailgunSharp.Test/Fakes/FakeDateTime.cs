using System;
using MailgunSharp.Wrappers;

namespace MailgunSharp.Test.Fakes
{
  public class FakeDateTime : IDateTimeProvider
  {
    public DateTime Now
    {
      get
      {
        return new DateTime(1970, 1, 1, 0, 0 , 0);
      }
    }

    public DateTime UtcNow
    {
      get
      {
        return new DateTime(1970, 1, 1, 0, 0, 0, 0);
      }
    }
  }
}