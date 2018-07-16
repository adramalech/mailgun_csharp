namespace MailgunSharp.Request
{
  public interface IQueryString
  {
    /// <summary>
    /// The current count of appended query string parameters.
    /// </summary>
    /// <value>int</value>
    int Count { get; }

    /// <summary>
    /// Append a value with a variable name as a parameter to the querystring if not null or empty.
    /// </summary>
    /// <param name="var">The varaible name of the parameter to be appended.</param>
    /// <param name="val">The value of the parameter to be appended.</param>
    /// <typeparam name="T">The value's type.</typeparam>
    bool AppendIfNotNullOrEmpty<T>(string var, T val) where T : struct;

    /// <summary>
    /// Append a value with a variable name as a parameter to the querystring if not null or empty.
    /// </summary>
    /// <param name="var">The varaible name of the parameter to be appended.</param>
    /// <param name="val">The value of the parameter to be appended.</param>
    bool AppendIfNotNullOrEmpty(string var, string val);
  }
}
