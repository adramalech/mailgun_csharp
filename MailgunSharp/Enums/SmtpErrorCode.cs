namespace MailgunSharp.Enums
{
  public enum SmtpErrorCode
  {
    /// <summary>
    /// The SMTP service is not available; the server is closing the transmission channel.
    /// </summary>
    SERVICE_IS_NOT_AVAILABLE = 421,

    /// <summary>
    /// The destination mailbox is in use.
    /// </summary>
    MAILBOX_UNAVAILABLE = 450,

    /// <summary>
    /// The SMTP service cannot complete the request. This error can occur if the client's
    /// IP address cannot be resolved (that is, a reverse lookup failed). You can also
    /// receive this error if the client domain has been identified as an open relay
    /// or source for unsolicited email (spam). For details, see RFC 2505, which is available
    /// at http://www.ietf.org.
    /// </summary>
    INTERNAL_SERVER_ERROR = 451,

    /// <summary>
    /// The SMTP service does not have sufficient storage to complete the request.
    /// </summary>
    SERVER_INSUFFICENT_STORAGE = 452,

    /// <summary>
    /// The client was not authenticated or is not allowed to send mail using
    /// the specified SMTP host.
    /// </summary>
    CLIENT_NOT_PERMITTED = 454,

    /// <summary>
    /// The server is temporarily unable to accomodate one or more of the
    /// parameters associated with a MAIL FROM or RCPT TO command.
    /// </summary>
    SERVER_UNABLE_TO_EXECUTE_COMMAND = 455,

    /// <summary>
    /// The SMTP service does not recognize the specified command.
    /// </summary>
    COMMAND_NOT_RECOGNIZED = 500,

    /// <summary>
    /// The syntax used to specify a command or parameter is incorrect.
    /// </summary>
    SYNTAX_ERROR_COMMAND_ARGUMENTS = 501,

    /// <summary>
    /// The SMTP service does not implement the specified command.
    /// </summary>
    COMMAND_NOT_IMPLEMENTED = 502,

    /// <summary>
    /// The commands were sent in the incorrect sequence.
    /// </summary>
    BAD_SEQUENCE_OF_COMMANDS = 503,

    /// <summary>
    /// The SMTP service does not implement the specified command parameter.
    /// </summary>
    COMMAND_PARAMETERS_NOT_IMPLEMENTED = 504,

    /// <summary>
    /// The SMTP service does not accept mail.
    /// </summary>
    DUMMY_SERVER_HOST_WONT_ACCEPT = 521,

    /// <summary>
    /// The SMTP server is configured to accept only TLS connections, and the SMTP client
    /// is attempting to connect by using a non-TLS connection. The solution is for the
    /// user to set EnableSsl to true on the SMTP Client.
    /// </summary>
    MUST_ISSUE_STARTTLS_FIRST = 530,

    /// <summary>
    /// The recipient address rejected a message: normally, it's an error caused by an anti-spam filter.
    /// </summary>
    MESSAGE_UNDELIVERABLE_POLICY = 541,

    /// <summary>
    /// The destination mailbox was not found or could not be accessed.
    /// </summary>
    USER_MAILBOX_UNAVAILABLE = 550,

    /// <summary>
    /// The user mailbox is not located on the receiving server. You should resend using
    /// the supplied address information.
    /// </summary>
    USER_NOT_ON_SERVER = 551,

    /// <summary>
    /// The message is too large to be stored in the destination mailbox.
    /// </summary>
    EXCEEDS_STORAGE_ALLOCATION = 552,

    /// <summary>
    /// The syntax used to specify the destination mailbox is incorrect.
    /// </summary>
    MAILBOX_NAME_INVALID = 553,

    /// <summary>
    /// The transaction failed.
    /// </summary>
    TRANSACTION_FAILED = 554,

    /// <summary>
    /// The outgoing SMTP server is not properly registering the email address used in either FROM or TO settings.
    /// </summary>
    ADDRESS_FORMAT_NOT_RECOGNIZED = 555,

    /// <summary>
    /// The recipient's SMTP server refuses to accept the message, permanently.
    /// </summary>
    RECIVING_SERVER_REJECTION_MESSAGE = 556
  }
}
