using System;
using System.Text;

namespace MailgunSharp.Extensions
{
  public static class StringBuilderExtensions
  {
    public static bool AddIfNotNullEmptyWhitespace(this StringBuilder stb, string str)
    {
      if (str.IsNullEmptyWhitespace())
      {
        return false;
      }

      stb.Append(str);

      return true;
    }

    public static bool IsEmpty(this StringBuilder stb)
    {
      return (stb.Length < 1);
    }
  }
}
