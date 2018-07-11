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
    /// <summary>
    /// A valid email address for the mailing list.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    private readonly MailAddress address;
    public MailAddress Address
    {
      get
      {
        return address;
      }
    }

    /// <summary>
    /// The name of the mailing list.
    /// </summary>
    /// <value>String</value>
    private readonly string name;
    public string Name
    {
      get
      {
        return name;
      }
    }

    /// <summary>
    /// A description of the mailing list.
    /// </summary>
    /// <value>String</value>
    private readonly string description;
    public string Description
    {
      get
      {
        return description;
      }
    }

    /// <summary>
    /// The level of access for a user to interface with this mailing list.
    /// </summary>
    /// <value>Access Level type.</value>
    private readonly AccessLevel accessLevel;
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
      jsonObject["access_level"] = getAccessLevelName(this.accessLevel);

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
        new KeyValuePair<string, string>("access_level", getAccessLevelName(this.accessLevel))
      };

      return content;
    }

    /// <summary>
    /// Get the name of the access level type.
    /// </summary>
    /// <param name="accessLevel">The access level type.</param>
    /// <returns>Name of the access level type.</returns>
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
