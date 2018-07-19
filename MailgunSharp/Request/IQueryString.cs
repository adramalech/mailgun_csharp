using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("MailgunSharp.Test")]

namespace MailgunSharp.Request
{
  internal interface IQueryString
  {
    /// <summary>
    /// The current count of appended query string parameters.
    /// </summary>
    /// <value>int</value>
    int Count { get; }

    /// <summary>
    /// Append a value with a variable name as a parameter to the querystring if not null or empty.
    /// </summary>
    /// <param name="variable">The varaible name of the parameter to be appended.</param>
    /// <param name="value">The value of the parameter to be appended.</param>
    bool AppendIfNotNullOrEmpty(string variable, string value);
  }
}
