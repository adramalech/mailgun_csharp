using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using MailgunSharp.Extensions;

namespace MailgunSharp.MailingLists
{
  public sealed class Member : IMember
  {
    private readonly MailAddress address;

    /// <summary>
    /// The email address of the mailing list member.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    public MailAddress Address
    {
      get
      {
        return this.address;
      }
    }

    private readonly string name;

    /// <summary>
    /// The name of the mailing list member.
    /// </summary>
    /// <value>string</value>
    public string Name
    {
      get
      {
        return this.name;
      }
    }

    private readonly JObject vars;

    /// <summary>
    /// JSON-encoded dictionary string with arbitrary parameters.
    /// </summary>
    /// <value>Json Object</value>
    public JObject Vars
    {
      get
      {
        return this.vars;
      }
    }

    private readonly bool subscribed;

    /// <summary>
    /// Is the member subscribed to the mailing list or not.
    /// </summary>
    /// <value>boolean</value>
    public bool Subscribed
    {
      get
      {
        return this.subscribed;
      }
    }

    private readonly bool upsert;

    /// <summary>
    /// Update the member if present in the mailing list, or
    /// raise an error if duplicate member found in the list.
    ///
    ///   True - allow member to be updated if duplicate member found.
    ///
    ///   False - raise error if duplicate member found. (default)
    /// </summary>
    /// <value>boolean</value>
    public bool Upsert
    {
      get
      {
        return this.upsert;
      }
    }

    /// <summary>
    /// Create an instance of the member class.
    /// </summary>
    /// <param name="address">The email address of the member.</param>
    /// <param name="name">The optional name of the member.</param>
    /// <param name="vars">The optional custom variables as a json object.</param>
    /// <param name="subscribed">Is the member subscribed to the mailing list. defaults to true.</param>
    /// <param name="upsert">True, update duplicate member found in list; false, raise an error if duplicate member found in list. Default false.</param>
    public Member(MailAddress address, string name = "", JObject vars = null,  bool subscribed = true, bool upsert = false)
    {
      if (this.address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      this.address = address;
      this.name = name;
      this.vars = vars;
      this.subscribed = subscribed;
      this.upsert = upsert;
    }

    /// <summary>
    /// Get the Member object as a json object to submit in an http request.
    /// </summary>
    /// <returns>json object</returns>
    public JObject ToJson()
    {
      var jsonObject = new JObject
      {
        ["address"] = this.address.ToString(),
        ["name"] = this.name,
        ["vars"] = this.vars.ToString(Formatting.None),
        ["subscribed"] = this.subscribed.ToYesNo(),
        ["upsert"] = this.upsert.ToYesNo()
      };

      return jsonObject;
    }

    /// <summary>
    /// Get the Member object as a form content to submit in an http request.
    /// </summary>
    /// <returns>List of key-value pairs of strings.</returns>
    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("address", this.address.ToString()),
        new KeyValuePair<string, string>("name", this.name),
        new KeyValuePair<string, string>("vars", this.vars.ToString(Formatting.None)),
        new KeyValuePair<string, string>("subscribed", this.subscribed.ToYesNo()),
        new KeyValuePair<string, string>("upsert", this.upsert.ToYesNo())
      };

      return content;
    }
  }
}
