using System;
using System.Net.Mail;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using MailgunSharp.Extensions;

namespace MailgunSharp.Messages
{
  public sealed class MessageBuilder : IMessageBuilder
  {
    /// <summary>
    /// The message to be built.
    /// </summary>
    private IMessage message;

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
    /// The size of the current message.
    /// </summary>
    private long messageSize;

    /// <summary>
    /// The number of recipients, "to", added to the message.
    /// </summary>
    private int recipientCount;

    /// <summary>
    /// The number of recipient variables added to the message.
    /// </summary>
    private int recipientVarCount;

    /// <summary>
    /// Create an instance of the message builder class.
    /// </summary>
    public MessageBuilder()
    {
      message = new Message();
      messageSize = 0;
      recipientCount = 0;
      recipientVarCount = 0;
    }

    /// <summary>
    /// Add a file to be attached to the message.
    /// </summary>
    /// <param name="attachment">A file attachment object with the file contents and filename.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddAttachment(IFileAttachment attachment)
    {
      if (attachment == null)
      {
        throw new ArgumentNullException("Attachment cannot be null!");
      }

      if (message.FileAttachments == null)
      {
        message.FileAttachments = new Collection<IFileAttachment>();
      }

      if (messageSize + attachment.Data.Length > MAX_TOTAL_MESSAGE_SIZE)
      {
        throw new ArgumentOutOfRangeException("Cannot exceed total message size of 25MB!");
      }

      messageSize += attachment.Data.Length;

      message.FileAttachments.Add(attachment);

      return this;
    }

    /// <summary>
    /// Add a file information to be read at a later time and added as a message attachment.
    /// </summary>
    /// <param name="attachment">The file info of the file to be attached.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddAttachment(FileInfo attachment)
    {
      if (attachment == null)
      {
        throw new ArgumentNullException("Attachment cannot be null!");
      }

      if (message.Attachments == null)
      {
        message.Attachments = new Collection<FileInfo>();
      }

      if (messageSize + attachment.Length > MAX_TOTAL_MESSAGE_SIZE)
      {
        throw new ArgumentOutOfRangeException("Cannot exceed total message size of 25MB!");
      }

      messageSize += attachment.Length;

      message.Attachments.Add(attachment);

      return this;
    }

    /// <summary>
    /// Add a valid email address to be added to the "blind carbon copy" header.
    /// </summary>
    /// <param name="bcc">A valid email address to be added to the bcc list.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddBcc(MailAddress bcc)
    {
      if (bcc == null)
      {
        throw new ArgumentNullException("Bcc cannot be null!");
      }

      if (message.Bcc == null) {
        message.Bcc = new Collection<MailAddress>();
      }

      message.Bcc.Add(bcc);

      return this;
    }

    /// <summary>
    /// Add a valid email address to be added to the "carbon copy" header.
    /// </summary>
    /// <param name="cc">A valid email address to be added to the cc list.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddCc(MailAddress cc)
    {
      if (cc == null)
      {
        throw new ArgumentNullException("Cc cannot be null!");
      }

      if (message.Cc == null) {
        message.Cc = new Collection<MailAddress>();
      }

      message.Cc.Add(cc);

      return this;
    }

    /// <summary>
    /// Add the in-line image file information to be written into the message.
    /// </summary>
    /// <param name="image"></param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddInlineImage(FileInfo image)
    {
      if (image == null)
      {
        throw new ArgumentNullException("Image cannot be null!");
      }

      if (message.Inline == null)
      {
        message.Inline = new Collection<FileInfo>();
      }

      if (messageSize + image.Length > MAX_TOTAL_MESSAGE_SIZE)
      {
        throw new ArgumentOutOfRangeException("Cannot exceed total message size of 25MB!");
      }

      messageSize += image.Length;

      message.Inline.Add(image);

      return this;
    }

    /// <summary>
    /// Add the in-line image file attachment to be written into the message.
    /// </summary>
    /// <param name="image">The file attachment object with the file contents and filename.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddInlineImage(IFileAttachment image)
    {
      if (image == null)
      {
        throw new ArgumentNullException("Image cannot be null!");
      }

      if (message.FileInline == null)
      {
        message.FileInline = new Collection<IFileAttachment>();
      }

      if (messageSize + image.Data.Length > MAX_TOTAL_MESSAGE_SIZE)
      {
        throw new ArgumentOutOfRangeException("Cannot exceed total message size of 25MB!");
      }

      messageSize += image.Data.Length;

      message.FileInline.Add(image);

      return this;
    }

    /// <summary>
    /// Add a recipient to the list of recipients for the "to" header.
    /// </summary>
    /// <param name="recipient">A valid recipient with a valid email address.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddRecipient(IRecipient recipient)
    {
      if (recipient == null)
      {
        throw new ArgumentNullException("Recipients cannot be null!");
      }

      var count = recipientCount + 1;

      if (count > MAX_RECIPIENT_SIZE)
      {
        throw new ArgumentOutOfRangeException("Maximum number of 1,000 recipients cannot be exceeded!");
      }

      if (message.To == null)
      {
        message.To = new Collection<IRecipient>();
      }

      if (message.RecipientVariables == null)
      {
        message.RecipientVariables = new JObject();
      }

      message.To.Add(recipient);
      recipientCount++;

      if (recipient.Variables != null)
      {
        message.RecipientVariables[recipient.Address] = recipient.Variables;
        recipientVarCount++;

        if (recipientVarCount > 0 && recipientCount != recipientVarCount)
        {
          throw new ArgumentOutOfRangeException("Did not have matching amount of recipient variables and recipients!");
        }
      }

      return this;
    }

    /// <summary>
    /// Add a list of recipients for the "to" header, will all contain valid email address for each recipient.
    /// </summary>
    /// <param name="recipients">The list of recipients to send the message to.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddRecipients(ICollection<IRecipient> recipients)
    {
      if (recipients == null)
      {
        throw new ArgumentNullException("Recipients cannot be null!");
      }

      if (message.To == null)
      {
        message.To = new Collection<IRecipient>();
      }

      if (message.RecipientVariables == null)
      {
        message.RecipientVariables = new JObject();
      }

      if (recipientCount + recipients.Count > MAX_RECIPIENT_SIZE)
      {
        throw new ArgumentOutOfRangeException("Maximum number of 1,000 recipients cannot be exceeded!");
      }

      foreach (var recipient in recipients)
      {
        message.To.Add(recipient);
        recipientCount++;

        if (recipient.Variables != null)
        {
          message.RecipientVariables[recipient.Address] = recipient.Variables;
          recipientVarCount++;

          if (recipientVarCount > 0 && recipientCount != recipientVarCount)
          {
            throw new ArgumentOutOfRangeException("Did not have matching amount of recipient variables and recipients!");
          }
        }
      }

      return this;
    }

    /// <summary>
    /// Set the email address for the "from" header.
    /// </summary>
    /// <param name="sender">The valid email address of the sender.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetFrom(MailAddress sender)
    {
      if (sender == null)
      {
        throw new ArgumentNullException("From cannot be null!");
      }

      message.From = sender;

      return this;
    }

    /// <summary>
    /// Set the html content of the message.
    /// </summary>
    /// <param name="html">A string containing html content.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetHtmlContentBody(string html)
    {
      if (html.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("HTML Body cannot be null!");
      }

      var sizeInBytes = (long)(html.Length * sizeof(char));

      if (exceedsMaxMessageSize(sizeInBytes))
      {
        throw new ArgumentOutOfRangeException("Cannot exceed total message size of 25MB!");
      }

      messageSize += sizeInBytes;

      message.Html = html;

      return this;
    }

    /// <summary>
    /// Set the subject of the message.
    /// </summary>
    /// <param name="subject"></param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetSubject(string subject)
    {
      if (subject.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Subject cannot be null!");
      }

      var sizeInBytes = (long)(subject.Length * sizeof(char));

      if (exceedsMaxMessageSize(sizeInBytes))
      {
        throw new ArgumentOutOfRangeException("Cannot exceed total message size of 25MB!");
      }

      message.Subject = subject;

      return this;
    }

    /// <summary>
    /// Enable sending message in test mode.
    /// </summary>
    /// <param name="testMode">True will set message to test mode and false will not use test mode.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetTestMode(bool testMode)
    {
      message.TestMode = testMode;

      return this;
    }

    /// <summary>
    /// Set the text content of the message.
    /// </summary>
    /// <param name="text">A string of plain text.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetTextContentBody(string text)
    {
      if (text.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Text Body cannot be null!");
      }

      message.Text = text;

      return this;
    }

    /// <summary>
    /// Add a tag to the message to be used to organize the messages that are sent.
    /// </summary>
    /// <param name="tag">The tag name.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddTag(string tag)
    {
      if (tag.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Tag cannot be null!");
      }

      if (message.Tags == null)
      {
        message.Tags = new Collection<string>();
      }

      message.Tags.Add(tag);

      return this;
    }

    /// <summary>
    /// Set the tracking of the message, will toggle per-message basis.
    /// </summary>
    /// <param name="enable">True will enable tracking, false will disable tracking.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetTracking(bool enable)
    {
      message.Tracking = enable;

      return this;
    }

    /// <summary>
    /// Set the tracking of the users opening the meessage.
    /// </summary>
    /// <param name="enable">True will enable tracking the users opening the message, false will disable open tracking.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetOpenTracking(bool enable)
    {
      message.TrackingOpens = enable;

      return this;
    }

    /// <summary>
    /// Set the tracking of the users clicking on links.
    /// </summary>
    /// <param name="enable">True will enable tracking the users clicking on links, false will disable click tracking.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetClickTracking(bool enable)
    {
      message.TrackingClicks = enable;

      return this;
    }

    /// <summary>
    /// Set the DKIM signatures on a per-message basis.
    /// </summary>
    /// <param name="enable">True will enable the DKIM signatures, false will disable DKIM signatures.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetDomainKeysIdentifiedMail(bool enable)
    {
      message.Dkim = enable;

      return this;
    }

    /// <summary>
    /// Add an arbitrary value to the custom header of the message.
    /// </summary>
    /// <param name="headerName">The name of the header.</param>
    /// <param name="value">The value of the header parameter.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddCustomHeader(string headerName, string value)
    {
      if (headerName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Custom header name cannot be null!");
      }

      if (value.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Custom header value cannot be null!");
      }

      if (message.CustomHeaders == null)
      {
        message.CustomHeaders = new Dictionary<string, string>();
      }

      message.CustomHeaders.Add(headerName, value);

      return this;
    }

    /// <summary>
    /// Send the message securely, if TLS connection cannot be established Mailgun will not deliver the message.
    /// If this flag isn't set it will still try to establish a secure connection, else will send in SMTP plaintext.
    /// </summary>
    /// <param name="enable">
    /// True, this requires the message only be sent over a TLS connection; false, Mailgun will still try
    /// and upgrade the connection, but if Mailgun cannot, the message will be delivered over plaintext SMTP connection.
    ///</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SendSecure(bool enable)
    {
      message.SendSecure = enable;

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
      message.SkipTlsVerification = enable;

      return this;
    }

    /// <summary>
    /// Set the delivery time of the message to be sent, will use UTC time.
    /// </summary>
    /// <param name="datetime">The datetime of the delivery as a UTC time.</param>
    /// <param name="tzInfo">Set the timezone you perfer to use.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetDeliveryTime(DateTime datetime, TimeZoneInfo tzInfo = null)
    {
      var now = DateTime.UtcNow;

      var localUtcDateTime = (tzInfo == null) ? datetime.ToUniversalTime() : TimeZoneInfo.ConvertTimeToUtc(datetime.ToUniversalTime(), tzInfo);

      var difference = localUtcDateTime.Subtract(now);

      if (difference.Days > 3)
      {
        throw new ArgumentOutOfRangeException("Delivery DateTime cannot exceed 3 days into the future!");
      }

      message.DeliveryTime = localUtcDateTime;

      return this;
    }

    /// <summary>
    /// Add custom JSON object to the message header.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <param name="value">The value of the variable.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder AddCustomData(string name, JObject value)
    {
      if (name.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Name cannot be null!");
      }

      if (value == null)
      {
        throw new ArgumentNullException("Value cannot be null!");
      }

      if (message.CustomData == null)
      {
        message.CustomData = new Dictionary<string, JObject>();
      }

      message.CustomData.Add(name, value);

      return this;
    }

    /// <summary>
    /// Add the Reply-To address as a custom header.
    /// </summary>
    /// <param name="replyTo">A valid email address to be used for the recipients to reply to.</param>
    /// <returns>The instance of the builder.</returns>
    public IMessageBuilder SetReplyTo(MailAddress replyTo)
    {
      if (replyTo == null)
      {
        throw new ArgumentNullException("replyTo cannot be null!");
      }

      if (message.CustomHeaders == null)
      {
        message.CustomHeaders = new Dictionary<string, string>();
      }

      message.CustomHeaders.Add("Reply-To", replyTo.Address);

      return this;
    }

    /// <summary>
    /// Get the instance of the message that was built.
    /// </summary>
    /// <returns>The instance of the message that was built.</returns>
    public IMessage Build()
    {
      return message;
    }

    /// <summary>
    /// Does the message exceed the 25MB size limit?
    /// </summary>
    /// <param name="size">The size of the current message.</param>
    /// <returns>True if it does exceed, false if it doesn't.</returns>
    private bool exceedsMaxMessageSize(long size)
    {
      return ((messageSize + size) > MAX_TOTAL_MESSAGE_SIZE);
    }

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

      if (lines == null || lines.Length < 1) {
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
