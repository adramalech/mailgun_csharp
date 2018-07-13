using System;
using System.Net.Mail;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Messages
{
  public interface IMessageBuilder
  {
    IMessageBuilder SetTestMode(bool testMode);
    IMessageBuilder SetFrom(MailAddress sender);
    IMessageBuilder AddRecipient(IRecipient recipient);
    IMessageBuilder AddRecipients(ICollection<IRecipient> recipients);
    IMessageBuilder AddCc(MailAddress cc);
    IMessageBuilder AddBcc(MailAddress bcc);
    IMessageBuilder SetReplyTo(MailAddress replyTo);
    IMessageBuilder SetSubject(string subject);
    IMessageBuilder SetTextContentBody(string text);
    IMessageBuilder SetHtmlContentBody(string html);
    IMessageBuilder SetDeliveryTime(DateTime datetime, TimeZoneInfo tzInfo = null);
    IMessageBuilder AddAttachment(IFileAttachment attachment);
    IMessageBuilder AddAttachment(FileInfo attachment);
    IMessageBuilder AddInlineImage(FileInfo image);
    IMessageBuilder AddInlineImage(IFileAttachment image);
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
