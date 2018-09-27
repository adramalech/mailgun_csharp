using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using MailgunSharp.Enums;
using MailgunSharp.Extensions;

namespace MailgunSharp.Domains
{
  public sealed class DomainRequest
  {
    private readonly string domainName;

    /// <summary>
    /// The domain name.
    /// </summary>
    /// <value>string</value>
    public string DomainName => this.domainName;

    private readonly string smtpPassword;

    /// <summary>
    /// Password for SMTP authentication.
    /// </summary>
    /// <value>System.Net.NetworkCredential</value>
    public string SmtpPassword => this.smtpPassword;

    private readonly SpamAction spamAction;

    /// <summary>
    /// The action the domain will have for handling spam.
    /// </summary>
    /// <value>SpamAction type.</value>
    public SpamAction SpamAction => this.spamAction;

    private readonly bool wildcard;

    /// <summary>
    /// Determines whether the domain will accept email for sub-domains.
    /// </summary>
    /// <value>boolean</value>
    public bool Wildcard => this.wildcard;

    private readonly bool forceDKIMAuthority;

    /// <summary>
    /// Is the domain the DKIM authority for itself,
    /// or does it inherit the same DKIM Authority of the root domain registered.
    /// </summary>
    /// <value>
    /// True, the domain will be DKIM authority for itself;
    /// false, will be the same DKIM Authority as the root domain registered.
    /// </value>
    public bool ForceDKIMAuthority => this.forceDKIMAuthority;

    /// <summary>
    /// Create an instance of the domain request.
    /// </summary>
    /// <param name="domainName">The domain name.</param>
    /// <param name="smtpPassword">The password for SMTP authentication.</param>
    /// <param name="spamAction">The action the domain will have when handling spam.</param>
    /// <param name="wildcard">Will the domain accept email for sub-domains.</param>
    /// <param name="forceDKIMAuthority">Is the domain itself the DKIM Authority, or will it inherit DKIM Authority from root registered domain.</param>
    /// <exception cref="FormatException">The domain name must be a correctly formatted, for reference check RFC 2396 and RFS 2732.</exception>
    /// <exception cref="ArgumentNullException">The domain name and smtp password are required parameters and cannot be null, empty, or whitespace.</exception>
    public DomainRequest(string domainName, string smtpPassword, SpamAction spamAction = SpamAction.DISABLED, bool wildcard = false, bool forceDKIMAuthority = false)
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(domainName), "DomainName cannot be null or empty!");
      }

      if (smtpPassword.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(smtpPassword), "Smtp Password cannot be null or empty!");
      }

      if (!this.isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
      }

      this.domainName = domainName;
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
      var jsonObject = new JObject
      {
        ["name"] = this.domainName,
        ["smtp_password"] = this.smtpPassword,
        ["spam_action"] = EnumLookup.GetSpamActionName(this.spamAction),
        ["wildcard"] = this.wildcard,
        ["force_dkim_authority"] = this.forceDKIMAuthority
      };

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
        new KeyValuePair<string, string>("name", this.domainName),
        new KeyValuePair<string, string>("smtp_password", this.smtpPassword),
        new KeyValuePair<string, string>("spam_action", EnumLookup.GetSpamActionName(this.spamAction)),
        new KeyValuePair<string, string>("wildcard", this.wildcard.ToString().ToLower()),
        new KeyValuePair<string, string>("force_dkim_authority", this.forceDKIMAuthority.ToString().ToLower())
      };

      return content;
    }

    /// <summary>
    /// Check if the hostname is valid format.
    /// </summary>
    /// <param name="hostname">The hostname to check.</param>
    /// <returns>True, if valid, false if not valid.</returns>
    private bool isHostnameValid(string hostname)
    {
      var result = Uri.TryCreate(hostname, UriKind.Absolute, out Uri uri);

      return result && uri.IsWellFormedOriginalString();
    }
  }
}