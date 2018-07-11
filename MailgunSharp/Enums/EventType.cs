namespace MailgunSharp.Enums
{
  /// <summary>
  /// A type of Mailgun Event that occurs throughout the Mailgun System.
  /// </summary>
  public enum EventType
  {
    /// <summary>
    /// Mailgun accepted the request to send/forward the email and the message has been placed in queue.
    /// </summary>
    ACCEPTED,

    /// <summary>
    /// Mailgun rejected the request to send/forward the email.
    /// </summary>
    DELIVERED,

    /// <summary>
    /// Mailgun could not deliver the email to the recipient email server.
    /// </summary>
    FAILED,

    /// <summary>
    /// The email recipient opened the email and enabled image viewing.
    /// </summary>
    OPENED,

    /// <summary>
    /// The email recipient clicked on a link in the email.
    /// </summary>
    CLICKED,

    /// <summary>
    /// The email recipient clicked on the unsubscribe link.
    /// </summary>
    UNSUBSCRIBED,

    /// <summary>
    /// The email recipient clicked on the spam complaint button within their email client.
    /// </summary>
    COMPLAINED,

    /// <summary>
    /// Mailgun has sotred an incoming message.
    /// </summary>
    STORED
  }
}