using System;
using System.Runtime.CompilerServices;
using NodaTime;
using NodaTime.Text;

namespace MailgunSharp.Extensions
{
  internal static class NodaTimeExtensions
  {
    public static string ToRfc2822DateFormat(this Instant instant)
    {
      var pattern = InstantPattern.CreateWithInvariantCulture("ddd d MMM yyyy HH:mm:ss 'GMT'");
      var result = pattern.Format(instant);
      return result;
    }
  }
}