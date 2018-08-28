using System;
using System.Text;
using System.Text.Encodings.Web;
using MailgunSharp.Extensions;

using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("MailgunSharp.Test")]

namespace MailgunSharp.Request
{
  internal sealed class QueryStringBuilder : IQueryStringBuilder
  {
    /// <summary>
    /// The String builder that will be used to build the querystring.
    /// </summary>
    private StringBuilder stb;

    private int count;

    /// <summary>
    /// The current count of appended query string parameters.
    /// </summary>
    /// <value>int</value>
    public int Count => this.count;

    /// <summary>
    /// Create an instance of query string class with new stringbuilder and a zero appended parameter count.
    /// </summary>
    public QueryStringBuilder()
    {
      this.stb = new StringBuilder();
      this.count = 0;
    }

    /// <summary>
    /// Append a value with a variable name as a parameter to the querystring if not null or empty.
    /// </summary>
    /// <param name="variable">The varaible name of the parameter to be appended.</param>
    /// <param name="value">The value of the parameter to be appended.</param>
    public IQueryStringBuilder Append(string variable, string value)
    {
      if (variable.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(variable), "Variable cannot be null or empty!");
      }

      if (value.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(value), "Value cannot be null or empty!");
      }

      var prefix = (this.stb.IsEmpty()) ? "?" : "&";

      this.stb.Append($"{prefix}{UrlEncoder.Default.Encode(variable)}={UrlEncoder.Default.Encode(value)}");

      this.count++;

      return this;
    }

    /// <summary>
    /// Return the built string result.
    /// </summary>
    /// <returns>The resulting string that was built.</returns>
    public string Build()
    {
      if (this.stb.IsEmpty())
      {
        throw new InvalidOperationException("Query string is empty!");
      }

      return this.stb.ToString();
    }

    /// <summary>
    /// Override the object ToString method to return the result of the string builder.
    /// </summary>
    /// <returns>string</returns>
    public override string ToString()
    {
      return this.stb.ToString();
    }
  }
}
