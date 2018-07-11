namespace MailgunSharp.Enums
{
  /// <summary>
  /// The types of spam filter on a domain account.
  /// </summary>
  public enum SpamAction
  {
    /// <summary>
    /// No Spam Filtering will occur for inbound messages.
    /// The default is disabled.
    /// </summary>
    DISABLED,

    /// <summary>
    /// Inbound spam messages will not be delivered.
    /// </summary>
    BLOCKED,

    /// <summary>
    /// Inbound messages will be tagged with a spam header.
    /// </summary>
    TAG
  }
}