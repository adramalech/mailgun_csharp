using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MailgunSharp.Enums;
using MailgunSharp.Extensions;
using MailgunSharp.Request;
using MailgunSharp.Wrappers;
using NodaTime;

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
    public Instant? Start { get; set; }

    /// <summary>
    /// The end date.
    /// Defaults to current datetime UTC.
    /// </summary>
    /// <value>DateTime UTC.</value>
    public Instant? End { get; set; }

    private NodaTimeProvider provider;

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
      this.EventTypes = new Collection<EventType>();
      this.provider = new NodaTimeProvider();
    }

    ///  <summary>
    ///  Returns the properties as a query string to be used in an HTTP request.
    ///
    ///  Datetime fields start and end window ranges will be converted into unix epoch format.
    ///  </summary>
    /// <exception cref="ArgumentOutOfRangeException">Starting Date cannot be after ending date for datetime range.</exception>
    /// <returns>string value as a http query string.</returns>
    public string ToQueryString()
    {
      var queryStringBuilder = new QueryStringBuilder();

      if (!this.End.HasValue )
      {
        this.End = this.provider.Now();
      }

      if (!this.Start.HasValue)
      {
        this.Start = this.provider.Now().Minus(NodaTime.Duration.FromDays(7));
      }

      if (this.Start.Value.CompareTo(this.End.Value) > 0)
      {
        throw new ArgumentOutOfRangeException(nameof(this.Start), "The starting date range window cannot be after the ending date range.");
      }

      if (this.Duration.HasValue && this.Resolution.HasValue)
      {
        queryStringBuilder.Append("duration", $"{this.Duration.Value}{EnumLookup.GetTimeResolutionName(this.Resolution.Value)}");
      }
      else
      {
        queryStringBuilder
          .Append("start", this.Start.Value.ToRfc2822DateFormat())
          .Append("end", this.End.Value.ToRfc2822DateFormat());
      }

      if (this.EventTypes != null && this.EventTypes.Count > 0)
      {
        foreach (var eventType in this.EventTypes)
        {
          queryStringBuilder.Append("event", EnumLookup.GetEventTypeName(eventType));
        }
      }

      return queryStringBuilder.ToString();
    }
  }
}
