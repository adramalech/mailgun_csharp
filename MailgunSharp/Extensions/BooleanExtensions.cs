using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("MailgunSharp.Test")]

namespace MailgunSharp.Extensions
{
  internal static class BooleanExtensions
  {
    /// <summary>
    /// Get a boolean as a lowercase "yes" or "no"
    /// </summary>
    /// <param name="flag">The boolean to get string.</param>
    /// <returns>String value of True will be "yes", and False, will be "no".</returns>
    public static string ToYesNo(this bool flag)
    {
      return (flag ? "yes" : "no");
    }
  }
}