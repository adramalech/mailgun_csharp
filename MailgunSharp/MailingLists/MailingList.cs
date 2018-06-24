using System;
using System.Text;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MailgunSharp.MailingLists
{
  public sealed class MailingList
  {
    private readonly MailAddress address;
    public MailAddress Address
    {
      get
      {
        return address;
      }
    }

    private readonly string name;
    public string Name
    {
      get
      {
        return name;
      }
    }

    private readonly string description;
    public string Description
    {
      get
      {
        return description;
      }
    }

    private readonly AccessLevel accessLevel;
    public AccessLevel AccessLevel
    {
      get
      {
        return accessLevel;
      }
    }

    public MailingList(MailAddress address, string name = "", string description = "", AccessLevel accessLevel = AccessLevel.READ_ONLY)
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      this.address = address;
      this.name = name;
      this.description = description;
      this.accessLevel = accessLevel;
    }

    public JObject ToJson()
    {
      var jsonObject = new JObject();

      jsonObject["address"] = this.address.ToString();
      jsonObject["name"] = this.name;
      jsonObject["description"] = this.description;
      jsonObject["access_level"] = getAccessLevelName(this.accessLevel);

      return jsonObject;
    }

    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("address", this.address.ToString()),
        new KeyValuePair<string, string>("name", this.name),
        new KeyValuePair<string, string>("description", this.description),
        new KeyValuePair<string, string>("access_level", getAccessLevelName(this.accessLevel))
      };

      return content;
    }

    private string getAccessLevelName(AccessLevel accessLevel)
    {
      var name = "";

      switch (accessLevel)
      {
        case AccessLevel.READ_ONLY:
          name = "readonly";
          break;
        case AccessLevel.MEMBERS:
          name = "members";
          break;
        case AccessLevel.EVERYONE:
          name = "everyone";
          break;
      }

      return name;
    }
  }
}
