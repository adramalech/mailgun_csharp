using System;

namespace MailgunSharp.Extensions
{
  public static class UriExtensions
  {
    /// <summary>
    /// Get the minimal hostname from a URI.
    /// </summary>
    /// <param name="uri">The valid uri of the domain name.</param>
    /// <returns>String representing just the minimal hostname.</returns>
    public static string GetHostname(this Uri uri)
    {
      if (uri == null)
      {
        return string.Empty;
      }

      return uri.Host.Replace("https://", string.Empty).Replace("http://", string.Empty).Replace("www.", string.Empty);
    }
  }
}