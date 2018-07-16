using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using MailgunSharp.Enums;
using MailgunSharp.Extensions;

namespace MailgunSharp.Domains
{
  public sealed class DomainRequest
  {
    private readonly Uri name;

    /// <summary>
    /// The domain name.
    /// </summary>
    /// <value>System.Net.Uri</value>
    public Uri Name
    {
      get
      {
        return name;
      }
    }

    private readonly string smtpPassword;

    /// <summary>
    /// Password for SMTP authentication.
    /// </summary>
    /// <value>System.Net.NetworkCredential</value>
    public string SmtpPassword
    {
      get
      {
        return smtpPassword;
      }
    }

    private readonly SpamAction spamAction;

    /// <summary>
    /// The action the domain will have for handling spam.
    /// </summary>
    /// <value>SpamAction type.</value>
    public SpamAction SpamAction
    {
      get
      {
        return spamAction;
      }
    }

    private readonly bool wildcard;

    /// <summary>
    /// Determines wherether the domain will accept email for sub-domains.
    /// </summary>
    /// <value>boolean</value>
    public bool Wildcard
    {
      get
      {
        return wildcard;
      }
    }

    private readonly bool forceDKIMAuthority;

    /// <summary>
    /// Is the domain the DKIM authority for itself,
    /// or does it inherit the same DKIM Authority of the root domain registered.
    /// </summary>
    /// <value>
    /// True, the domain will be DKIM authority for itself;
    /// false, will be the same DKIM Authority as the root domain registered.
    /// </value>
    public bool ForceDKIMAuthority
    {
      get
      {
        return forceDKIMAuthority;
      }
    }

    /// <summary>
    /// Create an instance of the domain request.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="smtpPassword">The password for SMTP authentication.</param>
    /// <param name="spamAction">The action the domain will have when handling spam.</param>
    /// <param name="wildcard">Will the domain accept email for sub-domains.</param>
    /// <param name="forceDKIMAuthority">Is the domain itself the DKIM Authority, or will it inherit DKIM Authority from root registered domain.</param>
    public DomainRequest(Uri name, string smtpPassword, SpamAction spamAction = SpamAction.DISABLED, bool wildcard = false, bool forceDKIMAuthority = false)
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      if (smtpPassword.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Smtp Password cannot be null or empty!");
      }

      this.name = name;
      this.smtpPassword = smtpPassword;
      this.spamAction = spamAction;
      this.wildcard = wildcard;
      this.forceDKIMAuthority = forceDKIMAuthority;
    }

    /// <summary>
    /// Get a json object representation of the domain request object.
    /// </summary>
    /// <returns>Json object</returns>
    public JObject ToJson()
    {
      var jsonObject = new JObject();

      jsonObject["name"] = this.name.GetHostname();
      jsonObject["smtp_password"] = this.smtpPassword;
      jsonObject["spam_action"] = EnumLookup.GetSpamActionName(this.spamAction);
      jsonObject["wildcard"] = this.wildcard;
      jsonObject["force_dkim_authority"] = this.forceDKIMAuthority;

      return jsonObject;
    }

    /// <summary>
    /// Get a form content representation of the domain request object.
    /// </summary>
    /// <returns>List of key-value string pairs.</returns>
    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("name", this.name.GetHostname()),
        new KeyValuePair<string, string>("smtp_password", this.smtpPassword),
        new KeyValuePair<string, string>("spam_action", EnumLookup.GetSpamActionName(this.spamAction)),
        new KeyValuePair<string, string>("wildcard", this.wildcard.ToString().ToLower()),
        new KeyValuePair<string, string>("force_dkim_authority", this.forceDKIMAuthority.ToString().ToLower())
      };

      return content;
    }
  }
}