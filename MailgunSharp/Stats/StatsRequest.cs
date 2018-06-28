using System;
using System.Text;
using System.Collections.Generic;

namespace MailgunSharp.Stats
{
  public sealed class StatsRequest : IStatsRequest
  {
    public TimeResolution? Resolution { get; set; }
    public int? Duration { get; set; }
    public ICollection<EventType> EventTypes { get; set; }
    public DateTime Start  { get; set; }
    public DateTime End  { get; set; }

    public StatsRequest()
    {
      this.Resolution = TimeResolution.DAY;
      this.End = DateTime.UtcNow;
      this.Start = this.End.AddDays(-7);
    }

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
