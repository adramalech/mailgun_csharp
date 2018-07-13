using System;
using System.Net.Mail;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Messages
{
  public sealed class MessageBuilder : IMessageBuilder
  {
    private IMessage message;

    private const long MAX_TOTAL_MESSAGE_SIZE = 25000000;

    private const int MAX_RECIPIENT_SIZE = 1000;

    private long messageSize;

    private int recipientCount;

    private int recipientVarCount;

    public MessageBuilder()
    {
      message = new Message();
      messageSize = 0;
      recipientCount = 0;
      recipientVarCount = 0;
    }

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

    public IMessageBuilder SetFrom(MailAddress sender)
    {
      if (sender == null)
      {
        throw new ArgumentNullException("From cannot be null!");
      }

      message.From = sender;

      return this;
    }

    public IMessageBuilder SetHtmlContentBody(string html)
    {
      if (checkStringIfNullOrEmpty(html))
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

    public IMessageBuilder SetSubject(string subject)
    {
      if (checkStringIfNullOrEmpty(subject))
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

    public IMessageBuilder SetTestMode(bool testMode)
    {
      message.TestMode = testMode;

      return this;
    }

    public IMessageBuilder SetTextContentBody(string text)
    {
      if (checkStringIfNullOrEmpty(text))
      {
        throw new ArgumentNullException("Text Body cannot be null!");
      }

      message.Text = text;

      return this;
    }

    public IMessageBuilder AddTag(string tag)
    {
      if (checkStringIfNullOrEmpty(tag))
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

    public IMessageBuilder SetTracking(bool enable)
    {
      message.Tracking = enable;

      return this;
    }

    public IMessageBuilder SetOpenTracking(bool enable)
    {
      message.TrackingOpens = enable;

      return this;
    }

    public IMessageBuilder SetClickTracking(bool enable)
    {
      message.TrackingClicks = enable;

      return this;
    }

    public IMessageBuilder SetDomainKeysIdentifiedMail(bool enable)
    {
      message.Dkim = enable;

      return this;
    }

    public IMessageBuilder AddCustomHeader(string headerName, string value)
    {
      if (checkStringIfNullOrEmpty(headerName))
      {
        throw new ArgumentNullException("Custom header name cannot be null!");
      }

      if (checkStringIfNullOrEmpty(value))
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

    public IMessageBuilder SendSecure(bool enable)
    {
      message.SendSecure = enable;

      return this;
    }

    public IMessageBuilder SkipSecureVerification(bool enable)
    {
      message.SkipTlsVerification = enable;

      return this;
    }

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

    public IMessageBuilder AddCustomData(string name, JObject value)
    {
      if (checkStringIfNullOrEmpty(name))
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

    public IMessage Build()
    {
      return message;
    }

    private bool checkStringIfNullOrEmpty(string str)
    {
      return (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
    }

    private bool exceedsMaxMessageSize(long size)
    {
      return (messageSize + size > MAX_TOTAL_MESSAGE_SIZE);
    }

    private bool isRfc2822LineFolded(string text)
    {
      if (checkStringIfNullOrEmpty(text) || text.Length <= 998)
      {
        return true;
      }

      var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

      if (lines == null || lines.Length < 1) {
        return true;
      }

      foreach (var line in lines)
      {
        if (line.Length > 998)
        {
          return false;
        }
      }

      return true;
    }
  }
}
