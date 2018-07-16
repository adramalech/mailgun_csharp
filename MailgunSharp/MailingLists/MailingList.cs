using System;
using System.Text;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using MailgunSharp.Enums;

namespace MailgunSharp.MailingLists
{
  public sealed class MailingList
  {
    private readonly MailAddress address;

    /// <summary>
    /// A valid email address for the mailing list.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    public MailAddress Address
    {
      get
      {
        return address;
      }
    }

    private readonly string name;

    /// <summary>
    /// The name of the mailing list.
    /// </summary>
    /// <value>String</value>
    public string Name
    {
      get
      {
        return name;
      }
    }

    private readonly string description;

    /// <summary>
    /// A description of the mailing list.
    /// </summary>
    /// <value>String</value>
    public string Description
    {
      get
      {
        return description;
      }
    }

    private readonly AccessLevel accessLevel;

    /// <summary>
    /// The level of access for a user to interface with this mailing list.
    /// </summary>
    /// <value>Access Level type.</value>
    public AccessLevel AccessLevel
    {
      get
      {
        return accessLevel;
      }
    }

    /// <summary>
    /// Create a mailing list will default to requiring only the mailing list's email address.
    ///
    /// Defaults access_level to readonly.
    /// </summary>
    /// <param name="address">The email address of the mailing list.</param>
    /// <param name="name">The name of the mailing list. Optional.</param>
    /// <param name="description">The description of the mailing list. Optional.</param>
    /// <param name="accessLevel">The access level settings of the mailing list. Defaults to readonly.</param>
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

    /// <summary>
    /// Return the mailing list object as json.
    /// </summary>
    /// <returns>A Json object.</returns>
    public JObject ToJson()
    {
      var jsonObject = new JObject();

      jsonObject["address"] = this.address.ToString();
      jsonObject["name"] = this.name;
      jsonObject["description"] = this.description;
      jsonObject["access_level"] = EnumLookup.GetAccessLevelName(this.accessLevel);

      return jsonObject;
    }

    /// <summary>
    /// Return the mailing list object as a list of key-value pairs.
    /// </summary>
    /// <returns>List of key value pairs.</returns>
    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("address", this.address.ToString()),
        new KeyValuePair<string, string>("name", this.name),
        new KeyValuePair<string, string>("description", this.description),
        new KeyValuePair<string, string>("access_level", EnumLookup.GetAccessLevelName(this.accessLevel))
      };

      return content;
    }
  }
}
