using NodaTime;

namespace MailgunSharp.Wrappers
{
  public interface INodaTimeProvider
  {
    /// <summary>
    /// Return an instance from gaining an instance of the SystemClock.
    /// </summary>
    /// <returns>The current instance in time.</returns>
    Instant Now();

    /// <summary>
    /// Return an instance of the system clock.
    /// </summary>
    /// <returns>NodaTime IClock instance.</returns>
    IClock GetSystemClockInstance();
  }
}