using System;
using System.Net.Mail;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Supression
{
  public interface IComplaintRequest
  {
    /// <summary>
    /// Valid email address.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    MailAddress Address { get; }

    /// <summary>
    /// Timestamp of a complaint event.
    /// </summary>
    /// <value>DateTime</value>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Get Complaint Request object represented as json object for http request.
    /// </summary>
    /// <returns>Json object</returns>
    JObject ToJson();

    /// <summary>
    /// Get Complaint Request object represented as key-value string pair form content for http request.
    /// </summary>
    /// <returns>List key-value string pairs</returns>
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}
