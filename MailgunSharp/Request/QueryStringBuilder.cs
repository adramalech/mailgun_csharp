using System;
using MailgunSharp.Extensions;

namespace MailgunSharp.Request
{
  internal sealed class QueryStringBuilder : IQueryStringBuilder
  {
    /// <summary>
    /// The instance of query string to be built.
    /// </summary>
    private IQueryString queryString;

    /// <summary>
    /// Create an instance of query string builder class.
    /// </summary>
    public QueryStringBuilder()
    {
      this.queryString = new QueryString();
    }

    /// <summary>
    /// Append a parameter to the querystring.
    /// </summary>
    /// <param name="var">The parameter variable name.</param>
    /// <param name="val">The value of the paramater.</param>
    /// <returns>The query string builder instance.</returns>
    public IQueryStringBuilder Append(string var, string val)
    {
      this.queryString.AppendIfNotNullOrEmpty(var, val);

      return this;
    }

    /// <summary>
    /// Return the instance of the querystring that was built.
    /// </summary>
    /// <returns>The querystring.</returns>
    public IQueryString Build()
    {
      return queryString;
    }
  }
}