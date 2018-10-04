using NodaTime;

namespace MailgunSharp.Wrappers
{
  public interface INodaTimeProvider
  {
    Instant Now();
  }
}