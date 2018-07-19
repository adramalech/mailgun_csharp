using System;
using System.Text;

using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("MailgunSharp.Test")]

namespace MailgunSharp.Extensions
{
  internal static class StringBuilderExtensions
  {
    /// <summary>
    /// If the string being added is not null, empty or whitespace append the string to the stringbuilder.
    /// </summary>
    /// <param name="stb">The string builder to add the string value to.</param>
    /// <param name="str">The string to be added.</param>
    /// <returns>True if string is appended to stringbuilder, false if not.</returns>
    public static bool AddIfNotNullEmptyWhitespace(this StringBuilder stb, string str)
    {
      if (str.IsNullEmptyWhitespace())
      {
        return false;
      }

      stb.Append(str);

      return true;
    }

    /// <summary>
    /// Is the string builder have length of 0.
    /// </summary>
    /// <param name="stb">The string builder instance to check.</param>
    /// <returns>True if the string builder is empty, false if not empty.</returns>
    public static bool IsEmpty(this StringBuilder stb)
    {
      return (stb.Length < 1);
    }
  }
}
