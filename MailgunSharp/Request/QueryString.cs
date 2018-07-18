using System;
using System.Text;
using System.Text.Encodings.Web;
using MailgunSharp.Extensions;

namespace MailgunSharp.Request
{
  internal sealed class QueryString : IQueryString
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
    public int Count
    {
      get
      {
        return count;
      }
    }

    /// <summary>
    /// Create an instance of query string class with new stringbuilder and a zero appended parameter count.
    /// </summary>
    public QueryString()
    {
      this.stb = new StringBuilder();
      this.count = 0;
    }

    /// <summary>
    /// Append a value with a variable name as a parameter to the querystring if not null or empty.
    /// </summary>
    /// <param name="variable">The varaible name of the parameter to be appended.</param>
    /// <param name="value">The value of the parameter to be appended.</param>
    public bool AppendIfNotNullOrEmpty(string variable, string value)
    {
      if (variable.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Variable cannot be null or empty!");
      }

      if (value.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Value cannot be null or empty!");
      }

      var prefix = (stb.IsEmpty()) ? "?" : "&";

      var wasAdded = stb.AddIfNotNullEmptyWhitespace($"{prefix}{UrlEncoder.Default.Encode(variable)}={UrlEncoder.Default.Encode(value)}");

      if (wasAdded)
      {
        this.count++;
      }

      return wasAdded;
    }

    /// <summary>
    /// Override the object ToString method to return the result of the string builder.
    /// </summary>
    /// <returns>string</returns>
    public override string ToString()
    {
      return stb.ToString();
    }
  }
}
