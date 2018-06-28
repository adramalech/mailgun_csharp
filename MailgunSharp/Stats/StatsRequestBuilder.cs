using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MailgunSharp.Stats
{
  public sealed class StatsRequestBuilder : IStatsRequestBuilder
  {
    private IStatsRequest statsRequest;

    public StatsRequestBuilder()
    {
      this.statsRequest = new StatsRequest();
    }

    public IStatsRequestBuilder AddEventType(EventType eventType)
    {
      if (this.statsRequest.EventTypes == null)
      {
        this.statsRequest.EventTypes = new Collection<EventType>();
      }

      this.statsRequest.EventTypes.Add(eventType);

      return this;
    }

    public IStatsRequestBuilder SetTimeResolution(TimeResolution resolution)
    {
      this.statsRequest.Resolution = resolution;

      return this;
    }

    public IStatsRequestBuilder SetTimeDuration(int duration)
    {
      this.statsRequest.Duration = duration;

      return this;
    }

    public IStatsRequestBuilder SetStartTime(DateTime startTime)
    {
      this.statsRequest.Start = startTime;

      return this;
    }

    public IStatsRequestBuilder SetEndTime(DateTime endTime)
    {
      this.statsRequest.End = endTime;

      return this;
    }

    public IStatsRequest Build()
    {
      return this.statsRequest;
    }
  }
}
