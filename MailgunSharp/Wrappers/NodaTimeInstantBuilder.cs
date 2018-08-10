using System;
using System.Runtime.InteropServices.ComTypes;
using NodaTime;

namespace MailgunSharp.Wrappers
{
  public class NodaTimeInstantBuilder : INodaTimeInstantBuilder
  {
    private readonly IClock clock;

    private Instant now;

    public NodaTimeInstantBuilder(IClock clock)
    {
      this.clock = clock;
      this.now = this.clock.GetCurrentInstant();
    }

    public INodaTimeInstantBuilder AddDays(int days)
    {
      var duration = Duration.FromDays(days);

      this.now = this.now.Plus(duration);

      return this;
    }

    public INodaTimeInstantBuilder SubtractDays(int days)
    {
      var duration = Duration.FromDays(days);

      this.now = this.now.Minus(duration);

      return this;
    }

    public INodaTimeInstantBuilder AddHours(int hours)
    {
      var duration = Duration.FromHours(hours);

      this.now = this.now.Plus(duration);

      return this;
    }

    public INodaTimeInstantBuilder SubtractHours(int hours)
    {
      var duration = Duration.FromHours(hours);

      this.now = this.now.Minus(duration);

      return this;
    }

    public INodaTimeInstantBuilder AddMinutes(int minutes)
    {
      var duration = Duration.FromMinutes(minutes);

      this.now = this.now.Plus(duration);

      return this;
    }

    public INodaTimeInstantBuilder SubtractMinutes(int minutes)
    {
      var duration = Duration.FromMinutes(minutes);

      this.now = this.now.Minus(duration);

      return this;
    }

    public INodaTimeInstantBuilder AddSeconds(int seconds)
    {
      var duration = Duration.FromMinutes(seconds);

      this.now = this.now.Plus(duration);

      return this;
    }

    public INodaTimeInstantBuilder SubtractSeconds(int seconds)
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