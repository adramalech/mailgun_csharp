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
  public interface IMailingList
  {
    /// <summary>
    /// A valid email address for the mailing list.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    MailAddress Address { get; }

    /// <summary>
    /// The name of the mailing list.
    /// </summary>
    /// <value>String</value>
    string Name { get; }

    /// <summary>
    /// A description of the mailing list.
    /// </summary>
    /// <value>String</value>
    string Description { get; }

    /// <summary>
    /// The level of access for a user to interface with this mailing list.
    /// </summary>
    /// <value>Access Level type.</value>
    AccessLevel AccessLevel { get; }

    /// <summary>
    /// Return the mailing list object as json.
    /// </summary>
    /// <returns>A Json object.</returns>
    JObject ToJson();

    /// <summary>
    /// Return the mailing list object as a list of key-value pairs.
    /// </summary>
    /// <returns>List of key value pairs.</returns>
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}
