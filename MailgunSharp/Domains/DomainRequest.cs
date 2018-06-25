using System;
using System.Security;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Domains
{
  public sealed class DomainRequest
  {
    private readonly Uri name;
    public Uri Name 
    {
      get
      {
        return name;
      }
    }

    private readonly NetworkCredential smtpPassword;
    public NetworkCredential SmtpPassword
    {
      get
      {
        return smtpPassword;
      }
    }

    private readonly SpamAction spamAction;
    public SpamAction SpamAction 
    {
      get
      {
        return spamAction;
      }
    }

    private readonly bool wildcard;
    public bool Wildcard
    {
      get
      {
        return wildcard;
      }
    }

    private readonly bool forceDKIMAuthority;
    public bool ForceDKIMAuthority
    {
      get
      {
        return forceDKIMAuthority;
      }
    }

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
      this.smtpPassword = new NetworkCredential(string.Empty, smtpPassword);
      this.spamAction = spamAction;
      this.wildcard = wildcard;
      this.forceDKIMAuthority = forceDKIMAuthority;
    }

    public JObject ToJson()
    {
      var jsonObject = new JObject();

      jsonObject["name"] = getHostname(this.name);
      jsonObject["smtp_password"] = this.smtpPassword.Password;
      jsonObject["spam_action"] = getSpamActionName(this.spamAction);
      jsonObject["wildcard"] = this.wildcard;
      jsonObject["force_dkim_authority"] = this.forceDKIMAuthority;

      return jsonObject;
    }

    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("name", getHostname(this.name)),
        new KeyValuePair<string, string>("smtp_password", this.smtpPassword.Password),
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