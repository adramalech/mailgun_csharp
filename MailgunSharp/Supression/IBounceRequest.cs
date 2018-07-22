using System;
using System.Net.Mail;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using MailgunSharp.Enums;

namespace MailgunSharp.Supression
{
  public interface IBounceRequest
  {
    /// <summary>
    /// A valid email address.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    MailAddress EmailAddress { get; }

    /// <summary>
    /// SMTP Error code.
    /// </summary>
    /// <value>SmtpErrorCode type.</value>
    SmtpErrorCode Code { get; }

    /// <summary>
    /// Error description.
    /// </summary>
    /// <value>string</value>
    string Error { get; }

    /// <summary>
    /// Timestamp of a bounce event.
    /// </summary>
    /// <value>DateTime</value>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Get the Bounce request object as json to be used in an http request.
    /// </summary>
    /// <returns>Json object</returns>
    JObject ToJson();

    /// <summary>
    /// Get the Bounce request object as form content list key-value string pair to be used in an http request.
    /// </summary>
    /// <returns>List key-value string pair</returns>
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}
