namespace MailgunSharp.Routes
{
  public interface IExpressionBuilder
  {
    /// <summary>
    /// Used to combine two expressions together with a conditional AND.
    /// </summary>
    /// <returns>An instance of the expression builder.</returns>
    IExpressionBuilder And(IPattern pattern1, IPattern pattern2);

    /// <summary>
    /// Used to combine two expressions together with a conditional OR.
    /// </summary>
    /// <returns>An instance of the expression builder.</returns>
    IExpressionBuilder Or(IPattern pattern1, IPattern pattern2);

    /// <summary>
    /// Get the built string that represents the expression.
    /// </summary>
    /// <returns>The expression as a string.</returns>
    string Build();
  }
}