using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MailgunSharp.Enums;

namespace MailgunSharp.Stats
{
  public sealed class StatsRequest : IStatsRequest
  {
    /// <summary>
    /// Type of time range. Will be joined with Duration period.
    /// If provided will override the start date.
    /// </summary>
    /// <value>Time Resolution type.</value>
    public TimeResolution? Resolution { get; set; }

    /// <summary>
    /// Period of time.  Will be joined with Resolution.
    /// If provided will override the start date.
    /// </summary>
    /// <value>Integer value.</value>
    public int? Duration { get; set; }

    /// <summary>
    /// The list of events to filter.
    /// </summary>
    /// <value>Event Type enum.</value>
    public ICollection<EventType> EventTypes { get; set; }

    /// <summary>
    /// The starting time.
    /// Defaults to 7 days, one week, from the current datetime.
    /// </summary>
    /// <value>DateTime UTC.</value>
    public DateTime Start  { get; set; }

    /// <summary>
    /// The end date.
    /// Defaults to current datetime UTC.
    /// </summary>
    /// <value>DateTime UTC.</value>
    public DateTime End  { get; set; }

    /// <summary>
    /// Create a new instance of stats request object.
    ///
    /// The default time resolution + duration is 7 DAYs, or one week, from current time.
    /// The default end datetime range is current datetime in UTC.
    /// Create an empty list of event types.
    /// </summary>
    public StatsRequest()
    {
      this.Resolution = TimeResolution.DAY;
      this.End = DateTime.UtcNow;
      this.Start = this.End.AddDays(-7);
      this.EventTypes = new Collection<EventType>();
    }

    /// <summary>
    /// Returns the properties as a query string to be used in an HTTP request.
    ///
    /// Datetime fields start and end window ranges will be converted into unix epoch format.
    /// </summary>
    /// <returns>string value as a http query string.</returns>
    public string ToQueryString()
    {
      var strBuilder = new StringBuilder();

      if (this.Duration.HasValue && this.Resolution.HasValue)
      {
        strBuilder.Append($"duration={this.Duration.Value}{getTimeResolutionName(this.Resolution.Value)}");
      }
      else
      {
        strBuilder.Append($"start={((DateTimeOffset)this.Start).ToUnixTimeSeconds()}");
        strBuilder.Append($"&end={((DateTimeOffset)this.End).ToUnixTimeSeconds()}");
      }

      if (this.EventTypes != null && this.EventTypes.Count > 0)
      {
        foreach (var eventType in this.EventTypes)
        {
          strBuilder.Append($"&event={getEventTypeName(eventType)}");
        }
      }

      return strBuilder.ToString();
    }

    /// <summary>
    /// Get the event type's name.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <returns>The name of the event type parameter as a string.</returns>
    private string getEventTypeName(EventType eventType)
    {
      var name = "";

      switch (eventType)
      {
        case EventType.ACCEPTED:
          name = "accepted";
          break;

        case EventType.CLICKED:
          name = "clicked";
          break;

        case EventType.COMPLAINED:
          name = "complained";
          break;

        case EventType.DELIVERED:
          name = "delivered";
          break;

        case EventType.FAILED:
          name = "failed";
          break;

        case EventType.OPENED:
          name = "opened";
          break;

        case EventType.STORED:
          name = "stored";
          break;

        case EventType.UNSUBSCRIBED:
          name = "unsubscribed";
          break;
      }

      return name;
    }

    /// <summary>
    /// Get the Time Resolution's name.
    /// </summary>
    /// <param name="resolution">The time resolution type.</param>
    /// <returns>The name of the time resolution parameter as a string.</returns>
    private string getTimeResolutionName(TimeResolution resolution)
    {
      var name = "";

      switch (resolution)
      {
        case TimeResolution.DAY:
          name = "d";
          break;

        case TimeResolution.HOUR:
          name = "h";
          break;

        case TimeResolution.MONTH:
          name = "m";
          break;
      }

      return name;
    }
  }
}
