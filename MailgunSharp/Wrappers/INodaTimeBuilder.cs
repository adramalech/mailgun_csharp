using NodaTime;

namespace MailgunSharp.Wrappers
{
  public interface INodaTimeBuilder
  {
    Instant Build();

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