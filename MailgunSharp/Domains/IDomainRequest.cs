using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using MailgunSharp.Enums;

namespace MailgunSharp.Domains
{
  public interface IDomainRequest
  {
    /// <summary>
    /// The domain name.
    /// </summary>
    /// <value>System.Net.Uri</value>
    Uri Name { get; }

    /// <summary>
    /// Password for SMTP authentication.
    /// </summary>
    /// <value>System.Net.NetworkCredential</value>
    NetworkCredential SmtpPassword { get; }

    /// <summary>
    /// The action the domain will have for handling spam.
    /// </summary>
    /// <value>SpamAction type.</value>
    SpamAction SpamAction { get; }

    /// <summary>
    /// Determines wherether the domain will accept email for sub-domains.
    /// </summary>
    /// <value>boolean</value>
    bool Wildcard { get; }

    /// <summary>
    /// Is the domain the DKIM authority for itself,
    /// or does it inherit the same DKIM Authority of the root domain registered.
    /// </summary>
    /// <value>
    /// True, the domain will be DKIM authority for itself;
    /// false, will be the same DKIM Authority as the root domain registered.
    /// </value>
    bool ForceDKIMAuthority { get; }

    /// <summary>
    /// Get a json object representation of the domain request object.
    /// </summary>
    /// <returns>Json object</returns>
    JObject ToJson();

    /// <summary>
    /// Get a form content representation of the domain request object.
    /// </summary>
    /// <returns>List of key-value string pairs.</returns>
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}