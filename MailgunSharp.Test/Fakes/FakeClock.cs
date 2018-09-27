using NodaTime;

namespace MailgunSharp.Test.Fakes
{
  public class FakeClock : IClock
  {
    public Instant GetCurrentInstant()
    {
      return Instant.FromUtc(1970, 1, 1, 0, 0, 0);
    }
  }
}