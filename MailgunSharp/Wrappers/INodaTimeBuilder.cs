using NodaTime;

namespace MailgunSharp.Wrappers
{
  public interface INodaTimeBuilder
  {
    /// <summary>
    /// Build the NodaTime instance based on order of operations.
    /// </summary>
    /// <returns>The resulting NodaTime instant which represents a datetime value in UTC.</returns>
    Instant Build();

    /// <summary>
    /// Add N number of days.
    /// </summary>
    /// <param name="days">The value of days to be added.</param>
    /// <returns>The instance of the builder.</returns>
    INodaTimeBuilder AddDays(int days);

    /// <summary>
    /// Subtract N number of days.
    /// </summary>
    /// <param name="days">The days to be subtracted.</param>
    /// <returns>The instance of the builder.</returns>
    INodaTimeBuilder SubtractDays(int days);

    /// <summary>
    /// Add N number of hours.
    /// </summary>
    /// <param name="hours">The hours to be added.</param>
    /// <returns>The instance of the builder.</returns>
    INodaTimeBuilder AddHours(int hours);

    /// <summary>
    /// Subtract N number of hours.
    /// </summary>
    /// <param name="hours">The hours to be subtracted.</param>
    /// <returns>The instance of the builder.</returns>
    INodaTimeBuilder SubtractHours(int hours);

    /// <summary>
    /// Add N number of minutes.
    /// </summary>
    /// <param name="minutes">The minutes to be added.</param>
    /// <returns>The instance of the builder.</returns>
    INodaTimeBuilder AddMinutes(int minutes);

    /// <summary>
    /// Subtract N number of minutes.
    /// </summary>
    /// <param name="minutes">The minutes to be subtracted.</param>
    /// <returns>The instance of the builder.</returns>
    INodaTimeBuilder SubtractMinutes(int minutes);

    /// <summary>
    /// Add N number of seconds.
    /// </summary>
    /// <param name="seconds">The seconds to be added.</param>
    /// <returns>The instance of the builder.</returns>
    INodaTimeBuilder AddSeconds(int seconds);

    /// <summary>
    /// Subtract N number of seconds.
    /// </summary>
    /// <param name="seconds">The seconds to be subtracted.</param>
    /// <returns>The instance of the builder.</returns>
    INodaTimeBuilder SubtractSeconds(int seconds);
  }
}