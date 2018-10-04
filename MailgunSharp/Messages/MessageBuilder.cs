using System;
using System.Net.Mail;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using MailgunSharp.Extensions;
using MailgunSharp.Wrappers;
using NodaTime;

namespace MailgunSharp.Messages
{
  public sealed class MessageBuilder : IMessageBuilder
  {
    /// <summary>
    /// The message to be built.
    /// </summary>
    private IMessage message;

    private NodaTimeProvider provider;

    /// <summary>
    /// The maximum size of a message in bytes, or 25MB.
    /// </summary>
    private const long MAX_TOTAL_MESSAGE_SIZE = 25000000;

    /// <summary>
    /// The maximum number of recipients allowed on a "to" MIME header for batch email sending.
    /// </summary>
    private const int MAX_RECIPIENT_SIZE = 1000;

    /// <summary>
    /// The maximum allowed character before a CRLF line ending must be added to fold the line.
    /// CRLF takes up two characters "CRLF" = \r\n.
    /// </summary>
    private const int RFC_2822_LINE_FOLD_LENGTH = 998;

    /// <summary>
    /// The size of the current this.message.
    /// </summary>
    private long messageSize;

    /// <summary>
    /// The number of recipients, "to", added to the this.message.
    /// </summary>
    private int recipientCount;

    /// <summary>
    /// The number of recipient variables added to the this.message.
    /// </summary>
    private int recipientVarCount;

    /// <summary>
    /// Create an instance of the message builder class.
    /// </summary>
    public MessageBuilder()
    {
      this.message = new Message();
      this.messageSize = 0;
      this.recipientCount = 0;
      this.recipientVarCount = 0;
      this.provider = new NodaTimeProvider();
    }

    /// <summary>
    /// Add a file to be attached to the this.message.
    /// </summary>
    /// <param name="attachment">A file attachment object with the file contents and filename.</param>
    /// <exception cref="ArgumentNullException">The attachment cannot be null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The maximum message size cannot exceed 25 MB in size.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddAttachment(IFileAttachment attachment)
    {
      if (attachment == null)
      {
        throw new ArgumentNullException(nameof(attachment), "Attachment cannot be null!");
      }

      if (this.message.FileAttachments == null)
      {
        this.message.FileAttachments = new Collection<IFileAttachment>();
      }

      var size = this.messageSize + attachment.Data.Length;

      if (size > MAX_TOTAL_MESSAGE_SIZE)
      {
        throw new ArgumentOutOfRangeException(nameof(this.messageSize), size, "Cannot exceed total message size of 25MB!");
      }

      this.messageSize += attachment.Data.Length;

      this.message.FileAttachments.Add(attachment);

      return this;
    }

    /// <summary>
    /// Add a file information to be read at a later time and added as a message attachment.
    /// </summary>
    /// <param name="attachment">The file info of the file to be attached.</param>
    /// <exception cref="ArgumentNullException">The attachment cannot be null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The maximum message size cannot exceed 25 MB in size.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddAttachment(FileInfo attachment)
    {
      if (attachment == null)
      {
        throw new ArgumentNullException(nameof(attachment), "Attachment cannot be null!");
      }

      if (this.message.Attachments == null)
      {
        this.message.Attachments = new Collection<FileInfo>();
      }

      var size = this.messageSize + attachment.Length;

      if (size > MAX_TOTAL_MESSAGE_SIZE)
      {
        throw new ArgumentOutOfRangeException(nameof(this.messageSize), size, "Cannot exceed total message size of 25MB!");
      }

      this.messageSize += attachment.Length;

      this.message.Attachments.Add(attachment);

      return this;
    }

    /// <summary>
    /// Add a valid email address to be added to the "blind carbon copy" header.
    /// </summary>
    /// <param name="bcc">A valid email address to be added to the bcc list.</param>
    /// <exception cref="ArgumentNullException">The bcc cannot be null or empty.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddBcc(MailAddress bcc)
    {
      if (bcc == null)
      {
        throw new ArgumentNullException(nameof(bcc), "Bcc cannot be null!");
      }

      if (this.message.Bcc == null) {
        this.message.Bcc = new Collection<MailAddress>();
      }

      this.message.Bcc.Add(bcc);

      return this;
    }

    /// <summary>
    /// Add a valid email address to be added to the "carbon copy" header.
    /// </summary>
    /// <param name="cc">A valid email address to be added to the cc list.</param>
    /// <exception cref="ArgumentNullException">The cc cannot be null or empty.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddCc(MailAddress cc)
    {
      if (cc == null)
      {
        throw new ArgumentNullException(nameof(cc), "Cc cannot be null!");
      }

      if (this.message.Cc == null) {
        this.message.Cc = new Collection<MailAddress>();
      }

      this.message.Cc.Add(cc);

      return this;
    }

    /// <summary>
    /// Add the in-line image file information to be written into the this.message.
    /// </summary>
    /// <param name="image"></param>
    /// <exception cref="ArgumentNullException">The inline image cannot be null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The maximum message size cannot of 25 MB cannot be exceeded.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddInlineImage(FileInfo image)
    {
      if (image == null)
      {
        throw new ArgumentNullException(nameof(image), "Image cannot be null!");
      }

      if (this.message.Inline == null)
      {
        this.message.Inline = new Collection<FileInfo>();
      }

      var size = this.messageSize + image.Length;

      if (size > MAX_TOTAL_MESSAGE_SIZE)
      {
        throw new ArgumentOutOfRangeException(nameof(this.messageSize), size, "Cannot exceed total message size of 25MB!");
      }

      this.messageSize += image.Length;

      this.message.Inline.Add(image);

      return this;
    }

    /// <summary>
    /// Add the in-line image file attachment to be written into the this.message.
    /// </summary>
    /// <param name="image">The file attachment object with the file contents and filename.</param>
    /// <exception cref="ArgumentNullException">Image cannot be null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The maximum message size cannot of 25 MB cannot be exceeded.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddInlineImage(IFileAttachment image)
    {
      if (image == null)
      {
        throw new ArgumentNullException(nameof(image), "Image cannot be null!");
      }

      if (this.message.FileInline == null)
      {
        this.message.FileInline = new Collection<IFileAttachment>();
      }

      var size = this.messageSize + image.Data.Length;

      if (size > MAX_TOTAL_MESSAGE_SIZE)
      {
        throw new ArgumentOutOfRangeException(nameof(this.messageSize), size, "Cannot exceed total message size of 25MB!");
      }

      this.messageSize += image.Data.Length;

      this.message.FileInline.Add(image);

      return this;
    }

    /// <summary>
    /// Add a recipient to the list of recipients for the "to" header.
    /// </summary>
    /// <param name="recipient">A valid recipient with a valid email address.</param>
    /// <exception cref="ArgumentNullException">Recipient cannot be null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The maximum number of recipients cannot exceed 1,000.  Also, recipients must match recipient variables, if provided.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddRecipient(IRecipient recipient)
    {
      if (recipient == null)
      {
        throw new ArgumentNullException(nameof(recipient), "Recipients cannot be null!");
      }

      var count = this.recipientCount + 1;

      if (count > MAX_RECIPIENT_SIZE)
      {
        throw new ArgumentOutOfRangeException(nameof(this.recipientCount), count, "Maximum number of 1,000 recipients cannot be exceeded!");
      }

      if (this.message.To == null)
      {
        this.message.To = new Collection<IRecipient>();
      }

      if (this.message.RecipientVariables == null)
      {
        this.message.RecipientVariables = new JObject();
      }

      this.message.To.Add(recipient);
      this.recipientCount++;

      if (recipient.Variables != null)
      {
        this.message.RecipientVariables[recipient.Address] = recipient.Variables;

        this.recipientVarCount++;

        if (this.recipientVarCount > 0 && this.recipientCount != this.recipientVarCount)
        {
          throw new ArgumentOutOfRangeException(nameof(this.message.RecipientVariables), this.recipientVarCount, "Did not have matching amount of recipient variables and recipients!");
        }
      }

      return this;
    }

    /// <summary>
    /// Add a list of recipients for the "to" header, will all contain valid email address for each recipient.
    /// </summary>
    /// <param name="recipients">The list of recipients to send the message to.</param>
    /// <exception cref="ArgumentNullException">Recipients cannot be null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The maximum number of recipients cannot exceed 1,000.  Also, recipients must match recipient variables, if provided.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddRecipients(ICollection<IRecipient> recipients)
    {
      if (recipients == null)
      {
        throw new ArgumentNullException(nameof(recipients), "Recipients cannot be null!");
      }

      if (this.message.To == null)
      {
        this.message.To = new Collection<IRecipient>();
      }

      if (this.message.RecipientVariables == null)
      {
        this.message.RecipientVariables = new JObject();
      }

      if (this.recipientCount + recipients.Count > MAX_RECIPIENT_SIZE)
      {
        throw new ArgumentOutOfRangeException(nameof(this.recipientCount), (this.recipientCount + recipients.Count), "Maximum number of 1,000 recipients cannot be exceeded!");
      }

      foreach (var recipient in recipients)
      {
        this.message.To.Add(recipient);
        this.recipientCount++;

        if (recipient.Variables != null)
        {
          this.message.RecipientVariables[recipient.Address] = recipient.Variables;
          this.recipientVarCount++;

          if (this.recipientVarCount > 0 && this.recipientCount != this.recipientVarCount)
          {
            throw new ArgumentOutOfRangeException(nameof(recipient.Variables), this.recipientVarCount, "Did not have matching amount of recipient variables and recipients!");
          }
        }
      }

      return this;
    }

    /// <summary>
    /// Set the email address for the "from" header.
    /// </summary>
    /// <param name="sender">The valid email address of the sender.</param>
    /// <exception cref="ArgumentNullException">The sender cannot be null or empty.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetFrom(MailAddress sender)
    {
      this.message.From = sender ?? throw new ArgumentNullException(nameof(sender), "From cannot be null!");

      return this;
    }

    /// <summary>
    /// Set the html content of the this.message.
    /// </summary>
    /// <param name="html">A string containing html content.</param>
    /// <exception cref="ArgumentNullException">Html message body cannot be empty or null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Message size cannot exceed 25 MBs.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetHtmlContentBody(string html)
    {
      if (html.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(html), "HTML Body cannot be null!");
      }

      var sizeInBytes = (long)(html.Length * sizeof(char));

      if (this.exceedsMaxMessageSize(sizeInBytes))
      {
        throw new ArgumentOutOfRangeException(nameof(html), sizeInBytes, "Cannot exceed total message size of 25MB!");
      }

      this.messageSize += sizeInBytes;

      this.message.Html = html;

      return this;
    }

    /// <summary>
    /// Set the subject of the this.message.
    /// </summary>
    /// <param name="subject"></param>
    /// <exception cref="ArgumentNullException">Subject cannot be null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Message cannot exceed a maximum size of 25 MB.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetSubject(string subject)
    {
      if (subject.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(subject), "Subject cannot be null!");
      }

      var sizeInBytes = (long)(subject.Length * sizeof(char));

      if (this.exceedsMaxMessageSize(sizeInBytes))
      {
        throw new ArgumentOutOfRangeException(nameof(subject), sizeInBytes, "Cannot exceed total message size of 25MB!");
      }

      this.message.Subject = subject;

      return this;
    }

    /// <summary>
    /// Enable sending message in test mode.
    /// </summary>
    /// <param name="testMode">True will set message to test mode and false will not use test mode.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetTestMode(bool testMode)
    {
      this.message.TestMode = testMode;

      return this;
    }

    /// <summary>
    /// Set the text content of the this.message.
    /// </summary>
    /// <param name="text">A string of plain text.</param>
    /// <exception cref="ArgumentNullException">The message body text cannot be null, empty, or whitespace.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetTextContentBody(string text)
    {
      if (text.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(text), "Text Body cannot be null!");
      }

      this.message.Text = text;

      return this;
    }

    /// <summary>
    /// Add a tag to the message to be used to organize the messages that are sent.
    /// </summary>
    /// <param name="tag">The tag name.</param>
    /// <exception cref="ArgumentNullException">The tag name cannot be null or empty.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddTag(string tag)
    {
      if (tag.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(tag), "Tag cannot be null!");
      }

      if (this.message.Tags == null)
      {
        this.message.Tags = new Collection<string>();
      }

      this.message.Tags.Add(tag);

      return this;
    }

    /// <summary>
    /// Set the tracking of the message, will toggle per-message basis.
    /// </summary>
    /// <param name="enable">True will enable tracking, false will disable tracking.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetTracking(bool enable)
    {
      this.message.Tracking = enable;

      return this;
    }

    /// <summary>
    /// Set the tracking of the users opening the meessage.
    /// </summary>
    /// <param name="enable">True will enable tracking the users opening the message, false will disable open tracking.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetOpenTracking(bool enable)
    {
      this.message.TrackingOpens = enable;

      return this;
    }

    /// <summary>
    /// Set the tracking of the users clicking on links.
    /// </summary>
    /// <param name="enable">True will enable tracking the users clicking on links, false will disable click tracking.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetClickTracking(bool enable)
    {
      this.message.TrackingClicks = enable;

      return this;
    }

    /// <summary>
    /// Set the DKIM signatures on a per-message basis.
    /// </summary>
    /// <param name="enable">True will enable the DKIM signatures, false will disable DKIM signatures.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetDomainKeysIdentifiedMail(bool enable)
    {
      this.message.Dkim = enable;

      return this;
    }

    /// <summary>
    /// Add an arbitrary value to the custom header of the this.message.
    /// </summary>
    /// <param name="headerName">The name of the header.</param>
    /// <param name="value">The value of the header parameter.</param>
    /// <exception cref="ArgumentNullException">Custom header name and value cannot be null or empty.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddCustomHeader(string headerName, string value)
    {
      if (headerName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(headerName), "Custom header name cannot be null!");
      }

      if (value.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(value), "Custom header value cannot be null!");
      }

      if (this.message.CustomHeaders == null)
      {
        this.message.CustomHeaders = new Dictionary<string, string>();
      }

      this.message.CustomHeaders.Add(headerName, value);

      return this;
    }

    /// <summary>
    /// Send the message securely, if TLS connection cannot be established Mailgun will not deliver the this.message.
    /// If this flag isn't set it will still try to establish a secure connection, else will send in SMTP plaintext.
    /// </summary>
    /// <param name="enable">
    /// True, this requires the message only be sent over a TLS connection; false, Mailgun will still try
    /// and upgrade the connection, but if Mailgun cannot, the message will be delivered over plaintext SMTP connection.
    ///</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SendSecure(bool enable)
    {
      this.message.SendSecure = enable;

      return this;
    }

    /// <summary>
    /// Sets if TLS certificate and hostname should not be verified when establishing a TLS connection
    /// and Mailgun will accept any certificate during delivery.
    /// </summary>
    /// <param name="enable">
    /// True, message sent will not verify certificate or hostname when establishing TLS connection to send;
    /// False, Mailgun will verify certificate and hostname. If either one cannot be verified, a TLS connection will not be established.
    ///</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SkipSecureVerification(bool enable)
    {
      this.message.SkipTlsVerification = enable;

      return this;
    }

    /// <summary>
    /// Set the delivery time of the message to be sent, will use UTC time.
    /// </summary>
    /// <param name="datetime">The datetime of the delivery as a UTC time.</param>
    /// <exception cref="ArgumentOutOfRangeException">Delivery date cannot exceed maximum 3 days into the future.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetDeliveryTime(Instant datetime)
    {
      var now = this.provider.Now();

      var difference = now.Minus(datetime);

      if (difference.Days > 3)
      {
        throw new ArgumentOutOfRangeException(nameof(datetime), datetime, "Delivery DateTime cannot exceed 3 days into the future!");
      }

      this.message.DeliveryTime = datetime;

      return this;
    }

    /// <summary>
    /// Add custom JSON object to the message header.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <param name="value">The value of the variable.</param>
    /// <exception cref="ArgumentNullException">Name and Value cannot be null or empty.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddCustomData(string name, JObject value)
    {
      if (name.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(name), "Name cannot be null!");
      }

      if (value == null)
      {
        throw new ArgumentNullException(nameof(value), "Value cannot be null!");
      }

      if (this.message.CustomData == null)
      {
        this.message.CustomData = new Dictionary<string, JObject>();
      }

      this.message.CustomData.Add(name, value);

      return this;
    }

    /// <summary>
    /// Add the Reply-To address as a custom header.
    /// </summary>
    /// <param name="replyTo">A valid email address to be used for the recipients to reply to.</param>
    /// <exception cref="ArgumentNullException">Reply to cannot be null or empty.</exception>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetReplyTo(MailAddress replyTo)
    {
      if (replyTo == null)
      {
        throw new ArgumentNullException(nameof(replyTo), "replyTo cannot be null!");
      }

      if (this.message.CustomHeaders == null)
      {
        this.message.CustomHeaders = new Dictionary<string, string>();
      }

      this.message.CustomHeaders.Add("Reply-To", replyTo.Address);

      return this;
    }

    /// <summary>
    /// Get the instance of the message that was built.
    /// </summary>
    /// <returns>The instance of the message that was built.</returns>
    public IMessage Build() => this.message;

    /// <summary>
    /// Does the message exceed the 25MB size limit?
    /// </summary>
    /// <param name="size">The size of the current this.message.</param>
    /// <returns>True if it does exceed, false if it doesn't.</returns>
    private bool exceedsMaxMessageSize(long size) => ((this.messageSize + size) > MAX_TOTAL_MESSAGE_SIZE);

    /// <summary>
    /// Does the lines of the recipient variables adhere to the RFC 2822 line folding.
    ///
    /// No line can exist in the message that is greater than 1,000 characters long including line ending.
    ///
    /// Make sure each 998 characters if greater than 998 character length has a line ending CRLF = \r\n.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private bool isRfc2822LineFolded(string text)
    {
      if (text.IsNullEmptyWhitespace() || text.Length <= RFC_2822_LINE_FOLD_LENGTH)
      {
        return true;
      }

      var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

      if (lines.Length < 1) {
        return true;
      }

      foreach (var line in lines)
      {
        if (line.Length > RFC_2822_LINE_FOLD_LENGTH)
        {
          return false;
        }
      }

      return true;
    }
  }
}
