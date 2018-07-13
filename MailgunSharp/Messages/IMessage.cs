using System;
using System.IO;
using System.Net.Mail;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Messages
{
  public interface IMessage
  {
    /// <summary>
    /// The valid email address of the sender.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    MailAddress From { get; set; }

    /// <summary>
    /// The email address and custom template variables of the recipients.
    /// </summary>
    /// <value>List of Recipients</value>
    ICollection<IRecipient> To { get; set; }

    /// <summary>
    /// List of valid email addresses for recipients of the carbon copy list.
    /// </summary>
    /// <value>List of System.Net.Mail.MailAddress</value>
    ICollection<MailAddress> Cc { get; set; }

    /// <summary>
    /// List of valid email addresses for recipients of the blind carbon copy list.
    /// </summary>
    /// <value>List of System.Net.Mail.MailAddress</value>
    ICollection<MailAddress> Bcc { get; set; }

    /// <summary>
    /// The subject of the message.
    /// </summary>
    /// <value>string</value>
    string Subject { get; set; }

    /// <summary>
    /// The text-formatted message body.
    /// </summary>
    /// <value>string</value>
    string Text { get; set; }

    /// <summary>
    /// The html-formatted message body.
    /// </summary>
    /// <value>string</value>
    string Html { get; set; }

    /// <summary>
    /// List of file attachments for the message.
    /// </summary>
    /// <value>File Attachment object.</value>
    ICollection<IFileAttachment> FileAttachments { get; set; }

    /// <summary>
    /// List of file attachments for the message.
    /// </summary>
    /// <value>System.IO.FileInfo</value>
    ICollection<FileInfo> Attachments { get; set; }

    /// <summary>
    /// List of inline message image attachments.
    /// </summary>
    /// <value>File Attachment object.</value>
    ICollection<IFileAttachment> FileInline { get; set; }

    /// <summary>
    /// List of inline message image attachments.
    /// </summary>
    /// <value>File Attachment object.</value>
    ICollection<FileInfo> Inline { get; set; }

    /// <summary>
    /// List of user defined tags to help catagorize outgoing email traffic based on some critera.
    /// </summary>
    /// <value>List of strings</value>
    ICollection<string> Tags { get; set; }

    /// <summary>
    /// Enable/Disable DKIM signatures on per-message basis.
    /// </summary>
    /// <value>boolean</value>
    bool Dkim { get; set; }

    /// <summary>
    /// What is the desired time of delivery.
    /// </summary>
    /// <value>DateTime</value>
    DateTime? DeliveryTime { get; set; }

    /// <summary>
    /// Enables sending in test mode.
    /// </summary>
    /// <value>boolean</value>
    bool TestMode { get; set; }

    /// <summary>
    /// Toggles tracking on a per-message basis.
    /// </summary>
    /// <value>boolean</value>
    bool Tracking { get; set; }

    /// <summary>
    /// Toggles clicks tracking on a per-message basis.
    /// </summary>
    /// <value>boolean</value>
    bool TrackingClicks { get; set; }

    /// <summary>
    /// Toggles opens tracking on a per-message basis.
    /// </summary>
    /// <value>boolean</value>
    bool TrackingOpens { get; set; }

    /// <summary>
    /// Sets if TLS certificate and hostname should not be verified when establishing a TLS connection
    /// and Mailgun will accept any certificate during delivery.
    /// </summary>
    /// <value>
    /// True, message sent will not verify certificate or hostname when establishing TLS connection to send;
    /// False, Mailgun will verify certificate and hostname. If either one cannot be verified, a TLS connection will not be established.
    ///</value>
    bool SkipTlsVerification { get; set; }

    /// <summary>
    /// Send the message securely, if TLS connection cannot be established Mailgun will not deliver the message.
    /// If this flag isn't set it will still try to establish a secure connection, else will send in SMTP plaintext.
    /// </summary>
    /// <value>
    /// True, this requires the message only be sent over a TLS connection; false, Mailgun will still try
    /// and upgrade the connection, but if Mailgun cannot, the message will be delivered over plaintext SMTP connection.
    ///</value>
    bool SendSecure { get; set; }

    /// <summary>
    /// Set custom variables per recipient that can be referenced in the message.
    /// </summary>
    /// <value>json object</value>
    JObject RecipientVariables { get; set; }

    /// <summary>
    /// Add an arbitrary custom MIME header value to the message.
    /// </summary>
    /// <value>dictionary of custom header strings.</value>
    IDictionary<string, string> CustomHeaders { get; set; }

    /// <summary>
    /// Add custom JSON data to the message.
    /// </summary>
    /// <value>dictionary of custom data json objects</value>
    IDictionary<string, JObject> CustomData { get; set; }

    /// <summary>
    /// Get the message as a list of key-value string pairs to be used in sending an http request.
    /// </summary>
    /// <returns>List of key-value string pairs.</returns>
    ICollection<KeyValuePair<string, string>> AsKeyValueCollection();

    /// <summary>
    /// Get the message as a httpcontent to be used in sending an http request.
    /// </summary>
    /// <returns>System.Net.Http.HttpContent</returns>
    HttpContent AsFormContent();
  }
}
