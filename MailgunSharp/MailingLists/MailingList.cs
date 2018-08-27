using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using MailgunSharp.Enums;

namespace MailgunSharp.MailingLists
{
  public sealed class MailingList
  {
    private readonly MailAddress emailAddress;

    /// <summary>
    /// A valid email address for the mailing list.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    public MailAddress EmailAddress => this.emailAddress;

    private readonly string name;

    /// <summary>
    /// The name of the mailing list.
    /// </summary>
    /// <value>String</value>
    public string Name => this.name;

    private readonly string description;

    /// <summary>
    /// A description of the mailing list.
    /// </summary>
    /// <value>String</value>
    public string Description => this.description;

    private readonly AccessLevel accessLevel;

    /// <summary>
    /// The level of access for a user to interface with this mailing list.
    /// </summary>
    /// <value>Access Level type.</value>
    public AccessLevel AccessLevel => accessLevel;

    /// <summary>
    /// Create a mailing list will default to requiring only the mailing list's email address.
    ///
    /// Defaults access_level to readonly.
    /// </summary>
    /// <param name="mailAddress">The email address of the mailing list.</param>
    /// <param name="name">The name of the mailing list. Optional.</param>
    /// <param name="description">The description of the mailing list. Optional.</param>
    /// <param name="accessLevel">The access level settings of the mailing list. Defaults to readonly.</param>
    public MailingList(MailAddress mailAddress, string name = "", string description = "", AccessLevel accessLevel = AccessLevel.READ_ONLY)
    {
      if (mailAddress == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      this.emailAddress = mailAddress;
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
      var jsonObject = new JObject
      {
        ["address"] = this.emailAddress.ToString(),
        ["name"] = this.name,
        ["description"] = this.description,
        ["access_level"] = EnumLookup.GetAccessLevelName(this.accessLevel)
      };

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
        new KeyValuePair<string, string>("address", this.emailAddress.ToString()),
        new KeyValuePair<string, string>("name", this.name),
        new KeyValuePair<string, string>("description", this.description),
        new KeyValuePair<string, string>("access_level", EnumLookup.GetAccessLevelName(this.accessLevel))
      };

      return content;
    }
  }
}
