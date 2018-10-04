using NodaTime;
using System.Collections.Generic;
using MailgunSharp.Enums;

namespace MailgunSharp.Stats
{
  /// <summary>
  /// Request to retrieve statstics of sepecific events.
  ///
  ///   Statistics retention policy:
  ///     - Hourly stats are preserved for a month.
  ///     - Daily stats are perserved for a year.
  ///     - Monthly stats are perserved throughout the lifespan of the domain.
  ///
  /// </summary>
  public interface IStatsRequest
  {
    /// <summary>
    /// Type of time range. Will be joined with Duration period.
    /// </summary>
    /// <value>Time Resolution type.</value>
    TimeResolution? Resolution { get; set; }

    /// <summary>
    /// Period of time.  Will be joined with Resolution.
    /// </summary>
    /// <value>Integer value.</value>
    int? Duration { get; set; }

    /// <summary>
    /// The list of events to filter.
    /// </summary>
    /// <value>Event Type enum.</value>
    ICollection<EventType> EventTypes { get; set; }

    /// <summary>
    /// The starting time.
    /// </summary>
    /// <value>DateTime UTC.</value>
    Instant? Start { get; set; }

    /// <summary>
    /// The end date.
    /// </summary>
    /// <value>DateTime UTC.</value>
    Instant? End { get; set; }

    /// <summary>
    /// Returns the properties as a query string to be used in an HTTP request.
    /// </summary>
    /// <returns>string value as a http query string.</returns>
    string ToQueryString();
  }
}
