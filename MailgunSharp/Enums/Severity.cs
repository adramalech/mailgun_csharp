namespace MailgunSharp.Enums
{
  /// <summary>
  /// Severity of Failure only used in Event type Failed.
  /// </summary>
  public enum Severity
  {
    /// <summary>
    /// A temporary rejection by an ESP.
    /// </summary>
    TEMPORARY,

    /// <summary>
    /// A permanent rejection by an ESP.
    /// </summary>
    PERMANENT
  }
}