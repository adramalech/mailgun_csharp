using System;
using System.Collections.ObjectModel;
using MailgunSharp.Enums;

namespace MailgunSharp.Stats
{
  public sealed class StatsRequestBuilder : IStatsRequestBuilder
  {
    /// <summary>
    /// The stats request to create and return when build() is called.
    /// </summary>
    private IStatsRequest statsRequest;

    /// <summary>
    /// Create a new instance of the stats request.
    /// </summary>
    public StatsRequestBuilder()
    {
      this.statsRequest = new StatsRequest();
    }

    /// <summary>
    /// Add an event type to the list of event types to retreive that events statistical data.
    /// </summary>
    /// <param name="eventType">The event type to be added to the list of event types.</param>
    /// <returns>The instance of builder.</returns>
    public IStatsRequestBuilder AddEventType(EventType eventType)
    {
      if (this.statsRequest.EventTypes == null)
      {
        this.statsRequest.EventTypes = new Collection<EventType>();
      }

      this.statsRequest.EventTypes.Add(eventType);

      return this;
    }

    /// <summary>
    /// Set the unit of time to be used in conjunction with the period of time.
    /// </summary>
    /// <param name="resolution">The unit of time measurement.</param>
    /// <returns>The instance of the builder.</returns>
    public IStatsRequestBuilder SetTimeResolution(TimeResolution resolution)
    {
      this.statsRequest.Resolution = resolution;

      return this;
    }

    /// <summary>
    /// Set the period of time to be used in conjunction with the resolution, unit of time.
    /// </summary>
    /// <param name="duration">An integer value greater than zero.</param>
    /// <returns>The instance of the builder.</returns>
    public IStatsRequestBuilder SetTimeDuration(int duration)
    {
      this.statsRequest.Duration = duration;

      return this;
    }

    /// <summary>
    /// Set the start datetime to be used as the start of the windowed datetime range.
    /// </summary>
    /// <param name="startTime">A datetime UTC value.</param>
    /// <param name="tzInfo">The optional timezone information for specific timezone awareness in the date.</param>
    /// <returns>The instance of the builder.</returns>
    public IStatsRequestBuilder SetStartTime(DateTime startTime, TimeZoneInfo tzInfo = null)
    {
      this.statsRequest.Start = (tzInfo == null) ? startTime.ToUniversalTime() : TimeZoneInfo.ConvertTimeToUtc(startTime.ToUniversalTime(), tzInfo);

      return this;
    }

    /// <summary>
    /// Set the end datetime to be used as the end of the windowed datetime range.
    /// </summary>
    /// <param name="endTime">A datetime UTC value.</param>
    /// <param name="tzInfo">The optional timezone information for specific timezone awareness in the date.</param>
    /// <returns>The instance of the builder.</returns>
    public IStatsRequestBuilder SetEndTime(DateTime endTime, TimeZoneInfo tzInfo = null)
    {
      this.statsRequest.End = (tzInfo == null) ? endTime.ToUniversalTime() : TimeZoneInfo.ConvertTimeToUtc(endTime.ToUniversalTime(), tzInfo);

      return this;
    }

    /// <summary>
    /// Build the request object.
    /// </summary>
    /// <returns>The instance of the stats request object the Request builder was building.</returns>
    public IStatsRequest Build() => this.statsRequest;
  }
}
