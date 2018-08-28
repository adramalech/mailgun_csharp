using System;
using NodaTime;

namespace MailgunSharp
{
  public sealed class NodaLocalDateTime
  {
    private readonly LocalDateTime localDateTime;

    private ZonedDateTime zonedDateTime;

    private readonly int year;

    public int Year => this.year;

    private readonly int month;

    public int Month => this.month;

    private readonly int day;

    public int Day => this.day;

    private readonly int hour;

    public int Hour => this.hour;

    private readonly int minute;

    public int Minute => this.minute;

    private readonly int second;

    public int Second => this.second;

    public NodaLocalDateTime(int year, int month, int day, int hour, int minute, int second)
    {
      this.localDateTime = new LocalDateTime(year, month, day, hour, minute, second);

      this.zonedDateTime = new ZonedDateTime(new Instant(), DateTimeZone.Utc);
    }

    internal LocalDateTime GetLocalDateTime()
    {
      return this.localDateTime;
    }
  }
}
