using System;

namespace MailgunSharp.Wrappers
{
  public interface INodaTimeInstantBuilder
  {
    DateTime Build();

    INodaTimeInstantBuilder AddDays(int days);
    INodaTimeInstantBuilder SubtractDays(int days);

    INodaTimeInstantBuilder AddHours(int hours);
    INodaTimeInstantBuilder SubtractHours(int hours);

    INodaTimeInstantBuilder AddMinutes(int minutes);
    INodaTimeInstantBuilder SubtractMinutes(int minutes);

    INodaTimeInstantBuilder AddSeconds(int seconds);
    INodaTimeInstantBuilder SubtractSeconds(int seconds);
  }
}