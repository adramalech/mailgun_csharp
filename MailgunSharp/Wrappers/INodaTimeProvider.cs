using System;

namespace MailgunSharp.Wrappers
{
  public interface INodaTimeProvider
  {
    DateTime Build();

    INodaTimeProvider AddDays(int days);
    INodaTimeProvider SubtractDays(int days);

    INodaTimeProvider AddHours(int hours);
    INodaTimeProvider SubtractHours(int hours);

    INodaTimeProvider AddMinutes(int minutes);
    INodaTimeProvider SubtractMinutes(int minutes);

    INodaTimeProvider AddSeconds(int seconds);
    INodaTimeProvider SubtractSeconds(int seconds);
  }
}