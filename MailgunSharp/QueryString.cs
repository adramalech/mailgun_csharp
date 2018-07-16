using System;
using System.Text;
using MailgunSharp.Extensions;

namespace MailgunSharp
{
  public class QueryString : IQueryString
  {
    private StringBuilder stb;

    private int count;
    public int Count
    {
      get
      {
        return count;
      }
    }

    public QueryString()
    {
      this.stb = new StringBuilder();
      this.count = 0;
    }

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

    public override string ToString()
    {
      return stb.ToString();
    }
  }
}
