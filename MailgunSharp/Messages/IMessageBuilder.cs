using NodaTime;
using System.Net.Mail;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Messages
{
  public interface IMessageBuilder
  {
    /// <summary>
    /// Enable sending message in test mode.
    /// </summary>
    /// <param name="testMode">True will set message to test mode and false will not use test mode.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder SetTestMode(bool testMode);

    /// <summary>
    /// Set the email address for the "from" header.
    /// </summary>
    /// <param name="sender">The valid email address of the sender.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder SetFrom(MailAddress sender);

    /// <summary>
    /// Add a recipient to the list of recipients for the "to" header.
    /// </summary>
    /// <param name="recipient">A valid recipient with a valid email address.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder AddRecipient(IRecipient recipient);

    /// <summary>
    /// Add a list of recipients for the "to" header, will all contain valid email address for each recipient.
    /// </summary>
    /// <param name="recipients">The list of recipients to send the message to.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder AddRecipients(ICollection<IRecipient> recipients);

    /// <summary>
    /// Add a valid email address to be added to the "carbon copy" header.
    /// </summary>
    /// <param name="cc">A valid email address to be added to the cc list.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder AddCc(MailAddress cc);

    /// <summary>
    /// Add a valid email address to be added to the "blind carbon copy" header.
    /// </summary>
    /// <param name="bcc">A valid email address to be added to the bcc list.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder AddBcc(MailAddress bcc);

    /// <summary>
    /// Add the Reply-To address as a custom header.
    /// </summary>
    /// <param name="replyTo">A valid email address to be used for the recipients to reply to.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder SetReplyTo(MailAddress replyTo);

    /// <summary>
    /// Set the subject of the message.
    /// </summary>
    /// <param name="subject"></param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder SetSubject(string subject);

    /// <summary>
    /// Set the text content of the message.
    /// </summary>
    /// <param name="text">A string of plain text.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder SetTextContentBody(string text);

    /// <summary>
    /// Set the html content of the message.
    /// </summary>
    /// <param name="html">A string containing html content.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder SetHtmlContentBody(string html);

    /// <summary>
    /// Set the delivery time of the message to be sent, will use UTC time.
    /// </summary>
    /// <param name="datetime">The datetime of the delivery as a UTC time.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder SetDeliveryTime(Instant datetime);

    /// <summary>
    /// Add a file to be attached to the message.
    /// </summary>
    /// <param name="attachment">A file attachment object with the file contents and filename.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder AddAttachment(IFileAttachment attachment);

    /// <summary>
    /// Add a file information to be read at a later time and added as a message attachment.
    /// </summary>
    /// <param name="attachment">The file info of the file to be attached.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder AddAttachment(FileInfo attachment);

    /// <summary>
    /// Add the in-line image file information to be written into the message.
    /// </summary>
    /// <param name="image"></param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder AddInlineImage(FileInfo image);

    /// <summary>
    /// Add the in-line image file attachment to be written into the message.
    /// </summary>
    /// <param name="image">The file attachment object with the file contents and filename.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder AddInlineImage(IFileAttachment image);

    /// <summary>
    /// Add a tag to the message to be used to organize the messages that are sent.
    /// </summary>
    /// <param name="tag">The tag name.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder AddTag(string tag);

    /// <summary>
    /// Set the tracking of the message, will toggle per-message basis.
    /// </summary>
    /// <param name="enable">True will enable tracking, false will disable tracking.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder SetTracking(bool enable);

    /// <summary>
    /// Set the tracking of the users opening the meessage.
    /// </summary>
    /// <param name="enable">True will enable tracking the users opening the message, false will disable open tracking.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder SetOpenTracking(bool enable);

    /// <summary>
    /// Set the tracking of the users clicking on links.
    /// </summary>
    /// <param name="enable">True will enable tracking the users clicking on links, false will disable click tracking.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder SetClickTracking(bool enable);

    /// <summary>
    /// Set the DKIM signatures on a per-message basis.
    /// </summary>
    /// <param name="enable">True will enable the DKIM signatures, false will disable DKIM signatures.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder SetDomainKeysIdentifiedMail(bool enable);

    /// <summary>
    /// Send the message securely, if TLS connection cannot be established Mailgun will not deliver the message.
    /// If this flag isn't set it will still try to establish a secure connection, else will send in SMTP plaintext.
    /// </summary>
    /// <param name="enable">
    /// True, this requires the message only be sent over a TLS connection; false, Mailgun will still try
    /// and upgrade the connection, but if Mailgun cannot, the message will be delivered over plaintext SMTP connection.
    ///</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder SendSecure(bool enable);

    /// <summary>
    /// Sets if TLS certificate and hostname should not be verified when establishing a TLS connection
    /// and Mailgun will accept any certificate during delivery.
    /// </summary>
    /// <param name="enable">
    /// True, message sent will not verify certificate or hostname when establishing TLS connection to send;
    /// False, Mailgun will verify certificate and hostname. If either one cannot be verified, a TLS connection will not be established.
    ///</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder SkipSecureVerification(bool enable);

    /// <summary>
    /// Add an arbitrary value to the custom header of the message.
    /// </summary>
    /// <param name="headerName">The name of the header.</param>
    /// <param name="value">The value of the header parameter.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder AddCustomHeader(string headerName, string value);

    /// <summary>
    /// Add custom JSON object to the message header.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <param name="value">The value of the variable.</param>
    /// <returns>The instance of the builder.</returns>
    IMessageBuilder AddCustomData(string name, JObject value);

    /// <summary>
    /// Get the instance of the message that was built.
    /// </summary>
    /// <returns>The instance of the message that was built.</returns>
    IMessage Build();
  }
}
