using NodaTime;

namespace MailgunSharp.Wrappers
{
  public sealed class NodaTimeProvider : INodaTimeProvider
  {
    /// <summary>
    /// Return an instance from gaining an instance of the SystemClock.
    /// </summary>
    /// <returns>The current instance in time.</returns>
    public Instant Now() => SystemClock.Instance.GetCurrentInstant();

    /// <summary>
    /// Return an instance of the system clock.
    /// </summary>
    /// <returns>NodaTime IClock instance.</returns>
    public IClock GetSystemClockInstance() => SystemClock.Instance;
  }
}