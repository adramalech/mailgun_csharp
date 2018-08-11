using System;
using System.Runtime.InteropServices.ComTypes;
using NodaTime;

namespace MailgunSharp.Wrappers
{
  public sealed class NodaTimeBuilder : INodaTimeBuilder
  {
    private readonly IClock clock;

    private Instant now;

    public NodaTimeBuilder(IClock clock)
    {
      this.clock = clock;
      this.now = this.clock.GetCurrentInstant();
    }

    public INodaTimeBuilder SetDateTimeUtc(int year, int month, int day, int hour, int minute, int second)
    {
      this.now = Instant.FromUtc(year, month, day, hour, minute, second);

      return this;
    }

    public INodaTimeBuilder SetDateTimeUtc(DateTime datetime)
    {
      this.now = Instant.FromDateTimeUtc(datetime);

      return this;
    }

    public INodaTimeBuilder AddDays(int days)
    {
      var duration = Duration.FromDays(days);

      this.now = this.now.Plus(duration);

      return this;
    }

    public INodaTimeBuilder SubtractDays(int days)
    {
      var duration = Duration.FromDays(days);

      this.now = this.now.Minus(duration);

      return this;
    }

    public INodaTimeBuilder AddHours(int hours)
    {
      var duration = Duration.FromHours(hours);

      this.now = this.now.Plus(duration);

      return this;
    }

    public INodaTimeBuilder SubtractHours(int hours)
    {
      var duration = Duration.FromHours(hours);

      this.now = this.now.Minus(duration);

      return this;
    }

    public INodaTimeBuilder AddMinutes(int minutes)
    {
      var duration = Duration.FromMinutes(minutes);

      this.now = this.now.Plus(duration);

      return this;
    }

    public INodaTimeBuilder SubtractMinutes(int minutes)
    {
      var duration = Duration.FromMinutes(minutes);

      this.now = this.now.Minus(duration);

      return this;
    }

    public INodaTimeBuilder AddSeconds(int seconds)
    {
      var duration = Duration.FromMinutes(seconds);

      this.now = this.now.Plus(duration);

      return this;
    }

    public INodaTimeBuilder SubtractSeconds(int seconds)
    {
      var duration = Duration.FromMinutes(seconds);

      this.now = this.now.Minus(duration);

      return this;
    }

    public DateTime Build()
    {
      return this.now.ToDateTimeUtc();
    }
  }
}