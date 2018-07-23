using System;
using System.Net.Mail;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MailgunSharp.Extensions;

namespace MailgunSharp.Messages
{
  public sealed class Message : IMessage
  {
    /// <summary>
    /// The valid email address of the sender.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    public MailAddress From { get; set; }

    /// <summary>
    /// The email address and custom template variables of the recipients.
    /// </summary>
    /// <value>List of Recipients</value>
    public ICollection<IRecipient> To { get; set; }

    /// <summary>
    /// List of valid email addresses for recipients of the carbon copy list.
    /// </summary>
    /// <value>List of System.Net.Mail.MailAddress</value>
    public ICollection<MailAddress> Cc { get; set; }

    /// <summary>
    /// List of valid email addresses for recipients of the blind carbon copy list.
    /// </summary>
    /// <value>List of System.Net.Mail.MailAddress</value>
    public ICollection<MailAddress> Bcc { get; set; }

    /// <summary>
    /// The subject of the message.
    /// </summary>
    /// <value>string</value>
    public string Subject { get; set; }

    /// <summary>
    /// The text-formatted message body.
    /// </summary>
    /// <value>string</value>
    public string Text { get; set; }

    /// <summary>
    /// The html-formatted message body.
    /// </summary>
    /// <value>string</value>
    public string Html { get; set; }

    /// <summary>
    /// List of file attachments for the message.
    /// </summary>
    /// <value>File Attachment object.</value>
    public ICollection<IFileAttachment> FileAttachments { get; set; }

    /// <summary>
    /// List of file attachments for the message.
    /// </summary>
    /// <value>System.IO.FileInfo</value>
    public ICollection<FileInfo> Attachments { get; set; }

    /// <summary>
    /// List of inline message image attachments.
    /// </summary>
    /// <value>File Attachment object.</value>
    public ICollection<IFileAttachment> FileInline { get; set; }

    /// <summary>
    /// List of inline message image attachments.
    /// </summary>
    /// <value>File Attachment object.</value>
    public ICollection<FileInfo> Inline { get; set; }

    /// <summary>
    /// List of user defined tags to help catagorize outgoing email traffic based on some critera.
    /// </summary>
    /// <value>List of strings</value>
    public ICollection<string> Tags { get; set; }

    /// <summary>
    /// Enable/Disable DKIM signatures on per-message basis.
    /// </summary>
    /// <value>boolean</value>
    public bool Dkim { get; set; }

    /// <summary>
    /// What is the desired time of delivery.  Delivery times can only be scheduled a maximum of three days in advance.
    /// </summary>
    /// <value>DateTime</value>
    public DateTime? DeliveryTime { get; set; }

    /// <summary>
    /// Enables sending in test mode.
    /// </summary>
    /// <value>boolean</value>
    public bool TestMode { get; set; }

    /// <summary>
    /// Toggles tracking on a per-message basis.
    /// </summary>
    /// <value>boolean</value>
    public bool Tracking { get; set; }

    /// <summary>
    /// Toggles clicks tracking on a per-message basis.
    /// </summary>
    /// <value>boolean</value>
    public bool TrackingClicks { get; set; }

    /// <summary>
    /// Toggles opens tracking on a per-message basis.
    /// </summary>
    /// <value>boolean</value>
    public bool TrackingOpens { get; set; }

    /// <summary>
    /// Set custom variables per recipient that can be referenced in the message.
    /// </summary>
    /// <value>json object</value>
    public JObject RecipientVariables { get; set; }

    /// <summary>
    /// Add an arbitrary custom header value to the message.
    /// </summary>
    /// <value>dictionary of custom header strings.</value>
    public IDictionary<string, string> CustomHeaders { get; set; }

    /// <summary>
    /// Add custom JSON data to the message.
    /// </summary>
    /// <value>dictionary of custom data json objects</value>
    public IDictionary<string, JObject> CustomData { get; set; }

    /// <summary>
    /// Sets if TLS certificate and hostname should not be verified when establishing a TLS connection
    /// and Mailgun will accept any certificate during delivery.
    /// </summary>
    /// <value>
    /// True, message sent will not verify certificate or hostname when establishing TLS connection to send;
    /// False, Mailgun will verify certificate and hostname. If either one cannot be verified, a TLS connection will not be established.
    ///</value>
    public bool SkipTlsVerification { get; set; }

    /// <summary>
    /// Send the message securely, if TLS connection cannot be established Mailgun will not deliver the message.
    /// If this flag isn't set it will still try to establish a secure connection, else will send in SMTP plaintext.
    /// </summary>
    /// <value>
    /// True, this requires the message only be sent over a TLS connection; false, Mailgun will still try
    /// and upgrade the connection, but if Mailgun cannot, the message will be delivered over plaintext SMTP connection.
    ///</value>
    public bool SendSecure { get; set; }

    /// <summary>
    /// The total message size including attachments cannot exceed 25MB.
    /// </summary>
    private const long MAX_TOTAL_MESSAGE_SIZE = 25000000;

    /// <summary>
    /// The number of allowed recipients in the "to" list has a maximum of 1,000 for a batch send.
    /// </summary>
    private const int MAX_RECIPIENT_SIZE = 1000;

    /// <summary>
    /// The maximum number of allowed tags to a domain. The number of allowable tags can be increased
    /// by sending a support email to mailgun support.
    /// </summary>
    private const int MAX_TAG_LIMIT = 4000;

    /// <summary>
    /// Create an instance of the Message class.
    /// </summary>
    public Message()
    {
      this.To = new Collection<IRecipient>();
      this.Cc = new Collection<MailAddress>();
      this.Bcc = new Collection<MailAddress>();
      this.Attachments = new Collection<FileInfo>();
      this.FileAttachments = new Collection<IFileAttachment>();
      this.Inline = new Collection<FileInfo>();
      this.FileInline = new Collection<IFileAttachment>();
      this.Tags = new Collection<string>();
      this.CustomHeaders = new Dictionary<string, string>();
      this.CustomData = new Dictionary<string, JObject>();
    }

    /// <summary>
    /// Get the message as a httpcontent to be used in sending an http request.
    /// </summary>
    /// <returns>System.Net.Http.HttpContent</returns>
    public HttpContent AsFormContent()
    {
      var formContent = new MultipartFormDataContent();

      if (this.Attachments != null)
      {
        foreach (var attachment in this.Attachments)
        {
          formContent.Add(new ByteArrayContent(File.ReadAllBytes(attachment.FullName)), "attachment", attachment.Name);
        }
      }

      if (this.FileAttachments != null)
      {
        foreach (var attachment in this.FileAttachments)
        {
          formContent.Add(new ByteArrayContent(attachment.Data), "attachment", attachment.FileName);
        }
      }

      if (this.Inline != null)
      {
        foreach (var image in this.Inline)
        {
          formContent.Add(new StreamContent(image.OpenRead()), "inline", image.Name);
        }
      }

      if (this.FileInline != null)
      {
        foreach (var image in this.FileInline)
        {
          formContent.Add(new ByteArrayContent(image.Data), "inline", image.FileName);
        }
      }

      foreach(var content in AsKeyValueCollection())
      {
        formContent.Add(new StringContent(content.Value), content.Key);
      }

      return formContent;
    }

    /// <summary>
    /// Get the message as a list of key-value string pairs to be used in sending an http request.
    /// </summary>
    /// <returns>List of key-value string pairs.</returns>
    public ICollection<KeyValuePair<string, string>> AsKeyValueCollection()
    {
      if (this.To == null)
      {
        throw new ArgumentNullException("Recipients cannot be empty!");
      }

      if (this.To.Count > MAX_RECIPIENT_SIZE)
      {
        throw new ArgumentOutOfRangeException("Maximum number of 1,000 recipients cannot be exceeded!");
      }

      if (this.RecipientVariables != null && this.RecipientVariables.Count != this.To.Count)
      {
        throw new ArgumentOutOfRangeException("Did not have matching amount of recipient variables and recipients!");
      }

      if (this.From == null)
      {
        throw new ArgumentNullException("Sender cannot be null!");
      }

      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("from", this.From.Address.ToString()),
        new KeyValuePair<string, string>("o:testmode", this.TestMode.ToYesNo()),
        new KeyValuePair<string, string>("o:tracking", this.Tracking.ToYesNo()),
        new KeyValuePair<string, string>("o:tracking-clicks",this.TrackingClicks.ToYesNo()),
        new KeyValuePair<string, string>("o:tracking-opens", this.TrackingOpens.ToYesNo()),
        new KeyValuePair<string, string>("o:require-tls", this.SendSecure.ToString()),
        new KeyValuePair<string, string>("o:skip-verfication", this.SkipTlsVerification.ToString()),
        new KeyValuePair<string, string>("o:dkim", this.Dkim.ToYesNo())
      };

      var addressList = new Collection<MailAddress>();

      foreach (var t in this.To)
      {
        addressList.Add(t.Address);
      }

      content.Add("to", generateCommaDelimenatedList(addressList));

      if (this.Cc != null)
      {
        content.Add("cc", generateCommaDelimenatedList(this.Cc));
      }

      if (this.Bcc != null)
      {
        content.Add("bcc", generateCommaDelimenatedList(this.Bcc));
      }

      if (!this.Subject.IsNullEmptyWhitespace())
      {
        content.Add("subject", this.Subject);
      }

      if (!this.Html.IsNullEmptyWhitespace())
      {
        content.Add("html", this.Html);
      }

      if (!this.Text.IsNullEmptyWhitespace())
      {
        content.Add("text", this.Text);
      }

      if (this.RecipientVariables != null)
      {
        content.Add("recipient-variables", this.RecipientVariables.ToString(Formatting.None));
      }

      if (this.Tags != null)
      {
        foreach (var tag in this.Tags)
        {
          content.Add("o:tag", tag);
        }
      }

      if (this.CustomHeaders != null)
      {
        foreach (var customHeader in this.CustomHeaders)
        {
          content.Add($"h:{customHeader.Key}", customHeader.Value);
        }
      }

      if (this.CustomData != null)
      {
        foreach (var data in this.CustomData)
        {
          content.Add($"v:{data.Key}", data.Value.ToString(Formatting.None));
        }
      }

      if (this.DeliveryTime != null && this.DeliveryTime.HasValue)
      {
        content.Add("o:deliverytime", ((DateTimeOffset)this.DeliveryTime.Value).ToUnixTimeSeconds().ToString());
      }

      return content;
    }

    /// <summary>
    /// Create a list of email addresses with commas seperating each address.
    /// </summary>
    /// <param name="addresses">List of email address to turn into a comma delimenated list of addresses.</param>
    /// <returns>string representing the comma delimenated addresses.</returns>
    private string generateCommaDelimenatedList(ICollection<MailAddress> addresses)
    {
      var addressList = new StringBuilder();

      var i = 1;
      var total = addresses.Count;

      var str = string.Empty;

      foreach (var address in addresses)
      {
        str = address.ToString();

        if (i < total)
        {
          str += ",";
        }

        addressList.Append(str);
        i++;
      }

      return addressList.ToString();
    }
  }
}
