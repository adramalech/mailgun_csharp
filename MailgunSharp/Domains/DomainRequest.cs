using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using MailgunSharp.Enums;

namespace MailgunSharp.Domains
{
  public sealed class DomainRequest
  {
    /// <summary>
    /// The domain name.
    /// </summary>
    /// <value>System.Net.Uri</value>
    private readonly Uri name;
    public Uri Name
    {
      get
      {
        return name;
      }
    }

    /// <summary>
    /// Password for SMTP authentication.
    /// </summary>
    /// <value>System.Net.NetworkCredential</value>
    private readonly string smtpPassword;
    public string SmtpPassword
    {
      get
      {
        return smtpPassword;
      }
    }

    /// <summary>
    /// The action the domain will have for handling spam.
    /// </summary>
    /// <value>SpamAction type.</value>
    private readonly SpamAction spamAction;
    public SpamAction SpamAction
    {
      get
      {
        return spamAction;
      }
    }

    /// <summary>
    /// Determines wherether the domain will accept email for sub-domains.
    /// </summary>
    /// <value>boolean</value>
    private readonly bool wildcard;
    public bool Wildcard
    {
      get
      {
        return wildcard;
      }
    }

    /// <summary>
    /// Is the domain the DKIM authority for itself,
    /// or does it inherit the same DKIM Authority of the root domain registered.
    /// </summary>
    /// <value>
    /// True, the domain will be DKIM authority for itself;
    /// false, will be the same DKIM Authority as the root domain registered.
    /// </value>
    private readonly bool forceDKIMAuthority;
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

      if (checkIfStringIsNullEmptyWhitespace(smtpPassword))
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

      jsonObject["name"] = getHostname(this.name);
      jsonObject["smtp_password"] = this.smtpPassword;
      jsonObject["spam_action"] = getSpamActionName(this.spamAction);
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
        new KeyValuePair<string, string>("name", getHostname(this.name)),
        new KeyValuePair<string, string>("smtp_password", this.smtpPassword),
        new KeyValuePair<string, string>("spam_action", getSpamActionName(this.spamAction)),
        new KeyValuePair<string, string>("wildcard", this.wildcard.ToString().ToLower()),
        new KeyValuePair<string, string>("force_dkim_authority", this.forceDKIMAuthority.ToString().ToLower())
      };

      return content;
    }

    private bool checkIfStringIsNullEmptyWhitespace(string str)
    {
      return (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
    }

    private string getSpamActionName(SpamAction spamAction)
    {
      var name = "";

      switch (spamAction)
      {
        case SpamAction.BLOCKED:
          name = "blocked";
          break;

        case SpamAction.DISABLED:
          name = "disabled";
          break;

        case SpamAction.TAG:
          name = "tag";
          break;
      }

      return name;
    }

    private string getHostname(Uri uri)
    {
      if (uri == null)
      {
        return string.Empty;
      }

      return uri.Host.Replace("https://", string.Empty).Replace("http://", string.Empty).Replace("www.", string.Empty);
    }
  }
}