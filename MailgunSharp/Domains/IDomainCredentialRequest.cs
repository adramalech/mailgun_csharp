using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Domains
{
  public interface IDomainCredentialRequest
  {
    /// <summary>
    /// The username for the smtp domain credentials.
    /// </summary>
    /// <value>string</value>
    string Username { get; }

    /// <summary>
    /// The password for the smtp domain credentials.
    /// </summary>
    /// <value>string</value>
    string Password { get; }

    /// <summary>
    /// Get the Domain Credential as a json object.
    /// </summary>
    /// <returns>A Json object of the Domain Credential request object.</returns>
    JObject ToJson();

    /// <summary>
    /// Get the Domain Credential as a form content key-value pair strings.
    /// </summary>
    /// <returns>List of keyvalue string pairs.</returns>
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}