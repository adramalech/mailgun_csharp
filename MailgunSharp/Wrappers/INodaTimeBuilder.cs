using System;

namespace MailgunSharp.Wrappers
{
  public interface INodaTimeBuilder
  {
    DateTime Build();

    INodaTimeBuilder SetDateTimeUtc(int year, int month, int day, int hour, int minute, int second);
    INodaTimeBuilder SetDateTimeUtc(DateTime datetime);

    INodaTimeBuilder AddDays(int days);
    INodaTimeBuilder SubtractDays(int days);

    INodaTimeBuilder AddHours(int hours);
    INodaTimeBuilder SubtractHours(int hours);

    INodaTimeBuilder AddMinutes(int minutes);
    INodaTimeBuilder SubtractMinutes(int minutes);

    INodaTimeBuilder AddSeconds(int seconds);
    INodaTimeBuilder SubtractSeconds(int seconds);
  }
}