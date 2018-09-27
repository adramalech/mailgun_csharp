using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("MailgunSharp.Test")]

namespace MailgunSharp.Extensions
{
  internal static class StringExtensions
  {
    /// <summary>
    /// Check if the string only is null, empty, or whitespace.
    /// </summary>
    /// <param name="str">The string to check.</param>
    /// <returns>True, if the string is only null, empty, or whitespace; false, if it isn't null, empty, or whitespace.</returns>
    public static bool IsNullEmptyWhitespace(this string str) => (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
  }
}