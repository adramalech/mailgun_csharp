using NodaTime;

namespace MailgunSharp.Wrappers
{
  public sealed class NodaTimeProvider : INodaTimeProvider
  {
    public Instant Now() => SystemClock.Instance.GetCurrentInstant();
  }
}