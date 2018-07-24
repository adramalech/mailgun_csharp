using System;
using System.Net.Mail;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MailgunSharp.Supression
{
  public interface IUnsubscriberRequest
  {
    /// <summary>
    /// A valid email address.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    MailAddress Address { get; }

    /// <summary>
    /// Unsubscribe from a specific tag.
    /// </summary>
    /// <value>string</value>
    string Tag { get; }

    /// <summary>
    /// Timestamp of an unsubscribe event.
    /// </summary>
    /// <value>DateTime</value>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Get Unsubscriber request object represented as a json object for http request.
    /// </summary>
    /// <returns>Json object</returns>
    JObject ToJson();

    /// <summary>
    /// Get Unsubscriber request object represented as a key-value string pair form content for http request.
    /// </summary>
    /// <returns>List key-value string pairs</returns>
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}
