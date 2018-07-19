using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("MailgunSharp.Test")]

namespace MailgunSharp.Request
{
  internal interface IQueryStringBuilder
  {
    /// <summary>
    /// Append a parameter to the querystring.
    /// </summary>
    /// <param name="variable">The parameter variable name.</param>
    /// <param name="value">The value of the paramater.</param>
    /// <returns>The query string builder instance.</returns>
    IQueryStringBuilder Append(string variable, string value);

    /// <summary>
    /// Return the instance of the querystring that was built.
    /// </summary>
    /// <returns>The querystring.</returns>
    IQueryString Build();
  }
}