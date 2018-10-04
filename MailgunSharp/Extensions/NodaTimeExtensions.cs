using NodaTime;
using NodaTime.Text;

namespace MailgunSharp.Extensions
{
  internal static class NodaTimeExtensions
  {
    /// <summary>
    ///  Turn a NodaTime instant into a datetime format of rfc2822 found on page #14.
    /// </summary>
    /// <param name="instant">The NodaTime instant to be used.</param>
    /// <returns>A string representing a datetime in RFC2822 format.</returns>
    public static string ToRfc2822DateFormat(this Instant instant)
    {
      var pattern = InstantPattern.CreateWithInvariantCulture("ddd d MMM yyyy HH:mm:ss 'GMT'");
      var result = pattern.Format(instant);
      return result;
    }
  }
}