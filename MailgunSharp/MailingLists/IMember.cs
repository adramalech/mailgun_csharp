using System.Net.Mail;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.MailingLists
{
  public interface IMember
  {
    /// <summary>
    /// The email address of the mailing list member.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    MailAddress Address { get; }

    /// <summary>
    /// The name of the mailing list member.
    /// </summary>
    /// <value>string</value>
    string Name { get; }

    /// <summary>
    /// JSON-encoded dictionary string with arbitrary parameters.
    /// </summary>
    /// <value>Json Object</value>
    JObject Vars { get; }

    /// <summary>
    /// Is the member subscribed to the mailing list or not.
    /// </summary>
    /// <value>boolean</value>
    bool Subscribed { get; }

    /// <summary>
    /// Update the member if present in the mailing list, or
    /// raise an error if duplicate member found in the list.
    ///
    ///   True - allow member to be updated if duplicate member found.
    ///
    ///   False - raise error if duplicate member found. (default)
    /// </summary>
    /// <value>boolean</value>
    bool Upsert { get; }

    /// <summary>
    /// Get the Member object as a json object to submit in an http request.
    /// </summary>
    /// <returns>json object</returns>
    JObject ToJson();

    /// <summary>
    /// Get the Member object as a form content to submit in an http request.
    /// </summary>
    /// <returns>List of key-value pairs of strings.</returns>
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}
