using NodaTime;
using MailgunSharp.Enums;

namespace MailgunSharp.Stats
{
  /// <summary>
  /// Build a stats request object.
  /// </summary>
  public interface IStatsRequestBuilder
  {
    /// <summary>
    /// Add an event type to the list of event types to retreive that events statistical data.
    /// </summary>
    /// <param name="eventType">The event type to be added to the list of event types.</param>
    /// <returns>The instance of builder.</returns>
    IStatsRequestBuilder AddEventType(EventType eventType);

    /// <summary>
    /// Set the unit of time to be used in conjunction with the period of time.
    /// </summary>
    /// <param name="resolution">The unit of time measurement.</param>
    /// <returns>The instance of the builder.</returns>
    IStatsRequestBuilder SetTimeResolution(TimeResolution resolution);

    /// <summary>
    /// Set the period of time to be used in conjunction with the resolution, unit of time.
    /// </summary>
    /// <param name="duration">An integer value greater than zero.</param>
    /// <returns>The instance of the builder.</returns>
    IStatsRequestBuilder SetTimeDuration(int duration);

    /// <summary>
    /// Set the start datetime to be used as the start of the windowed datetime range.
    /// </summary>
    /// <param name="startTime">A datetime UTC value.</param>
    /// <returns>The instance of the builder.</returns>
    IStatsRequestBuilder SetStartTime(Instant startTime);

    /// <summary>
    /// Set the end datetime to be used as the end of the windowed datetime range.
    /// </summary>
    /// <param name="endTime">A datetime UTC value.</param>
    /// <returns>The instance of the builder.</returns>
    IStatsRequestBuilder SetEndTime(Instant endTime);

    /// <summary>
    /// Build the request object.
    /// </summary>
    /// <returns>The instance of the stats request object the Request builder was building.</returns>
    IStatsRequest Build();
  }
}
