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
      try
      {
        var pattern = InstantPattern.CreateWithInvariantCulture("ddd d MMM yyyy HH:mm:ss '+0000'");
        var result = pattern.Format(instant);
        return result;
      }
      catch (Exception ex)
      {
        return null;
      }
    }
  }
}