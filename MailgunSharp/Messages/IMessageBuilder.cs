using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Messages
{
  public interface IMessageBuilder
  {
    IMessageBuilder SetTestMode(bool testMode);
    IMessageBuilder SetFrom(IRecipient sender);
    IMessageBuilder AddRecipient(IRecipient recipient);
    IMessageBuilder AddRecipients(ICollection<IRecipient> recipients);
    IMessageBuilder AddCc(IRecipient cc);
    IMessageBuilder AddBcc(IRecipient bcc);
    IMessageBuilder SetReplyTo(IRecipient replyTo);
    IMessageBuilder SetSubject(string subject);
    IMessageBuilder SetTextContentBody(string text);
    IMessageBuilder SetHtmlContentBody(string html);
    IMessageBuilder SetDeliveryTime(DateTime datetime, TimeZoneInfo tzInfo = null);
    IMessageBuilder AddAttachment(IFileAttachment attachment);
    IMessageBuilder AddAttachment(FileInfo attachment);
    IMessageBuilder AddInlineImage(FileInfo image);
    IMessageBuilder AddInlineImage(IFileAttachment image);
    IMessageBuilder AddCampaignId(string id);
    IMessageBuilder AddTag(string tag);
    IMessageBuilder SetTracking(bool enable);
    IMessageBuilder SetOpenTracking(bool enable);
    IMessageBuilder SetClickTracking(bool enable);
    IMessageBuilder SetDomainKeysIdentifiedMail(bool enable);
    IMessageBuilder SendSecure(bool enable);
    IMessageBuilder SkipSecureVerification(bool enable);
    IMessageBuilder AddCustomHeader(string headerName, string value);
    IMessageBuilder AddCustomData(string name, JObject value);
    IMessage Build();
  }
}
