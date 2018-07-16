using System;
using System.Net.Mail;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Messages
{
  public interface IMessageBuilder
  {
    /// <summary>
    ///
    /// </summary>
    /// <param name="testMode"></param>
    /// <returns></returns>
    IMessageBuilder SetTestMode(bool testMode);

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    IMessageBuilder SetFrom(MailAddress sender);

    /// <summary>
    ///
    /// </summary>
    /// <param name="recipient"></param>
    /// <returns></returns>
    IMessageBuilder AddRecipient(IRecipient recipient);

    /// <summary>
    ///
    /// </summary>
    /// <param name="recipients"></param>
    /// <returns></returns>
    IMessageBuilder AddRecipients(ICollection<IRecipient> recipients);

    /// <summary>
    ///
    /// </summary>
    /// <param name="cc"></param>
    /// <returns></returns>
    IMessageBuilder AddCc(MailAddress cc);

    /// <summary>
    ///
    /// </summary>
    /// <param name="bcc"></param>
    /// <returns></returns>
    IMessageBuilder AddBcc(MailAddress bcc);

    /// <summary>
    ///
    /// </summary>
    /// <param name="replyTo"></param>
    /// <returns></returns>
    IMessageBuilder SetReplyTo(MailAddress replyTo);

    /// <summary>
    ///
    /// </summary>
    /// <param name="subject"></param>
    /// <returns></returns>
    IMessageBuilder SetSubject(string subject);

    /// <summary>
    ///
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    IMessageBuilder SetTextContentBody(string text);

    /// <summary>
    ///
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    IMessageBuilder SetHtmlContentBody(string html);

    /// <summary>
    ///
    /// </summary>
    /// <param name="datetime"></param>
    /// <param name="tzInfo"></param>
    /// <returns></returns>
    IMessageBuilder SetDeliveryTime(DateTime datetime, TimeZoneInfo tzInfo = null);

    /// <summary>
    ///
    /// </summary>
    /// <param name="attachment"></param>
    /// <returns></returns>
    IMessageBuilder AddAttachment(IFileAttachment attachment);

    /// <summary>
    ///
    /// </summary>
    /// <param name="attachment"></param>
    /// <returns></returns>
    IMessageBuilder AddAttachment(FileInfo attachment);

    /// <summary>
    ///
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    IMessageBuilder AddInlineImage(FileInfo image);

    /// <summary>
    ///
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    IMessageBuilder AddInlineImage(IFileAttachment image);

    /// <summary>
    ///
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    IMessageBuilder AddTag(string tag);

    /// <summary>
    ///
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    IMessageBuilder SetTracking(bool enable);

    /// <summary>
    ///
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    IMessageBuilder SetOpenTracking(bool enable);

    /// <summary>
    ///
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    IMessageBuilder SetClickTracking(bool enable);

    /// <summary>
    ///
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    IMessageBuilder SetDomainKeysIdentifiedMail(bool enable);

    /// <summary>
    ///
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    IMessageBuilder SendSecure(bool enable);

    /// <summary>
    ///
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    IMessageBuilder SkipSecureVerification(bool enable);

    /// <summary>
    ///
    /// </summary>
    /// <param name="headerName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IMessageBuilder AddCustomHeader(string headerName, string value);

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IMessageBuilder AddCustomData(string name, JObject value);

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    IMessage Build();
  }
}
