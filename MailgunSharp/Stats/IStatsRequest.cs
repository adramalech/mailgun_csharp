using System;
using System.Collections.Generic;

namespace MailgunSharp.Stats
{
  public interface IStatsRequest
  {
    TimeResolution? Resolution { get; set; }
    int? Duration { get; set; }
    ICollection<EventType> EventTypes { get; set; }
    DateTime Start  { get; set; }
    DateTime End  { get; set; }

    string ToQueryString();
  }
}
