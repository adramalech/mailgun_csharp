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
    ///
    /// </summary>
    /// <value></value>
    ICollection<IFileAttachment> FileAttachments { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    ICollection<FileInfo> Attachments { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    ICollection<IFileAttachment> FileInline { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    ICollection<FileInfo> Inline { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value>List of strings</value>
    ICollection<string> Tags { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value>string</value>
    string CampaignId { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value>boolean</value>
    bool Dkim { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value>DateTime</value>
    DateTime? DeliveryTime { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value>boolean</value>
    bool TestMode { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value>boolean</value>
    bool Tracking { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value>boolean</value>
    bool TrackingClicks { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value>boolean</value>
    bool TrackingOpens { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value>boolean</value>
    bool SkipTlsVerification { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value>boolean</value>
    bool SendSecure { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value>json object</value>
    JObject RecipientVariables { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value>dictionary of custom header strings.</value>
    IDictionary<string, string> CustomHeaders { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value>dictionary of custom data json objects</value>
    IDictionary<string, JObject> CustomData { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <returns>List of key-value string pairs.</returns>
    ICollection<KeyValuePair<string, string>> AsKeyValueCollection();

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    HttpContent AsFormContent();
  }
}
