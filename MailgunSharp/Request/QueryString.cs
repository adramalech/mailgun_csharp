using System;
using System.Text;
using MailgunSharp.Extensions;

namespace MailgunSharp.Request
{
  public class QueryString : IQueryString
  {
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
    /// <param name="var">The varaible name of the parameter to be appended.</param>
    /// <param name="val">The value of the parameter to be appended.</param>
    /// <typeparam name="T">The value's type.</typeparam>
    public bool AppendIfNotNullOrEmpty<T>(string var, T val) where T : struct
    {
      if (var.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Variable cannot be null or empty!");
      }

      var wasAdded = stb.AddIfNotNullEmptyWhitespace((stb.IsEmpty()) ? $"{var}={val}" : $"&{var}={val}");

      if (wasAdded)
      {
        this.count++;
      }

      return wasAdded;
    }

    /// <summary>
    /// Append a value with a variable name as a parameter to the querystring if not null or empty.
    /// </summary>
    /// <param name="var">The varaible name of the parameter to be appended.</param>
    /// <param name="val">The value of the parameter to be appended.</param>
    public bool AppendIfNotNullOrEmpty(string var, string val)
    {
      if (var.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Variable cannot be null or empty!");
      }

      if (val.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Value cannot be null or empty!");
      }

      var wasAdded = stb.AddIfNotNullEmptyWhitespace((stb.IsEmpty()) ? $"{var}={val}" : $"&{var}={val}");

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
