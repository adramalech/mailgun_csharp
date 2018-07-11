using System;
using MailgunSharp.Enums;

namespace MailgunSharp.Stats
{
  public interface IStatsRequestBuilder
  {
    IStatsRequestBuilder AddEventType(EventType eventType);
    IStatsRequestBuilder SetTimeResolution(TimeResolution resolution);
    IStatsRequestBuilder SetTimeDuration(int duration);
    IStatsRequestBuilder SetStartTime(DateTime startTime);
    IStatsRequestBuilder SetEndTime(DateTime endTime);
    IStatsRequest Build();
  }
}
