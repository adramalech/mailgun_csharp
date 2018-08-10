using System;
using System.Runtime.InteropServices.ComTypes;
using NodaTime;

namespace MailgunSharp.Wrappers
{
  public class NodaTimeProvider : INodaTimeProvider
  {
    private readonly IClock clock;

    private Instant now;

    public NodaTimeProvider(IClock clock)
    {
      this.clock = clock;
      this.now = this.clock.GetCurrentInstant();
    }

    public INodaTimeProvider AddDays(int days)
    {
      var duration = Duration.FromDays(days);

      this.now = this.now.Plus(duration);

      return this;
    }

    public INodaTimeProvider SubtractDays(int days)
    {
      var duration = Duration.FromDays(days);

      this.now = this.now.Minus(duration);

      return this;
    }

    public INodaTimeProvider AddHours(int hours)
    {
      var duration = Duration.FromHours(hours);

      this.now = this.now.Plus(duration);

      return this;
    }

    public INodaTimeProvider SubtractHours(int hours)
    {
      var duration = Duration.FromHours(hours);

      this.now = this.now.Minus(duration);

      return this;
    }

    public INodaTimeProvider AddMinutes(int minutes)
    {
      var duration = Duration.FromMinutes(minutes);

      this.now = this.now.Plus(duration);

      return this;
    }

    public INodaTimeProvider SubtractMinutes(int minutes)
    {
      var duration = Duration.FromMinutes(minutes);

      this.now = this.now.Minus(duration);

      return this;
    }

    public INodaTimeProvider AddSeconds(int seconds)
    {
      var duration = Duration.FromMinutes(seconds);

      this.now = this.now.Plus(duration);

      return this;
    }

    public INodaTimeProvider SubtractSeconds(int seconds)
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