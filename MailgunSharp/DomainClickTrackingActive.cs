namespace MailgunSharp
{
  /// <summary>
  /// Click tracking settings for a Domain.
  /// </summary>
  public enum DomainClickTrackingActive
  {
    /// <summary>
    /// Links will be overwritten and pointed to Mailgun servers so Mailgun can track clicks.
    /// </summary>
    YES,

    /// <summary>
    /// Links will not be rewritten and Mailgun will not track clicks.
    /// </summary>
    NO,

    /// <summary>
    /// Links will only be rewritten in the HTML part of a message.
    /// </summary>
    HTML_ONLY
  }
}