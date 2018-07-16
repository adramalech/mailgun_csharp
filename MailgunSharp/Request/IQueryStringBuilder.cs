namespace MailgunSharp.Request
{
  public interface IQueryStringBuilder
  {
    /// <summary>
    /// Append a parameter to the querystring.
    /// </summary>
    /// <param name="var">The parameter variable name.</param>
    /// <param name="val">The value of the paramater.</param>
    /// <returns>The query string builder instance.</returns>
    IQueryStringBuilder Append(string var, string val);

    /// <summary>
    /// Return the instance of the querystring that was built.
    /// </summary>
    /// <returns>The querystring.</returns>
    IQueryString Build();
  }
}