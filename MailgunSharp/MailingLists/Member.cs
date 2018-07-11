using System;
using System.Text;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MailgunSharp.MailingLists
{
  public sealed class Member : IMember
  {
    /// <summary>
    /// The email address of the mailing list member.
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
    /// The name of the mailing list member.
    /// </summary>
    /// <value>string</value>
    private readonly string name;
    public string Name
    {
      get
      {
        return name;
      }
    }

    /// <summary>
    /// JSON-encoded dictionary string with arbitrary parameters.
    /// </summary>
    /// <value>Json Object</value>
    private readonly JObject vars;
    public JObject Vars
    {
      get
      {
        return vars;
      }
    }

    /// <summary>
    /// Is the member subscribed to the mailing list or not.
    /// </summary>
    /// <value>boolean</value>
    private readonly bool subscribed;
    public bool Subscribed
    {
      get
      {
        return subscribed;
      }
    }

    /// <summary>
    /// Update the member if present in the mailing list, or
    /// raise an error if duplicate member found in the list.
    ///
    ///   True - allow member to be updated if duplicate member found.
    ///
    ///   False - raise error if duplicate member found. (default)
    /// </summary>
    /// <value>boolean</value>
    private readonly bool upsert;
    public bool Upsert
    {
      get
      {
        return upsert;
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
      var jsonObject = new JObject();

      jsonObject["address"] = this.address.ToString();
      jsonObject["name"] = this.name;
      jsonObject["vars"] = this.vars.ToString(Formatting.None);
      jsonObject["subscribed"] = boolToYesNo(this.subscribed);
      jsonObject["upsert"] = boolToYesNo(this.upsert);

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
        new KeyValuePair<string, string>("subscribed", boolToYesNo(this.subscribed)),
        new KeyValuePair<string, string>("upsert", boolToYesNo(this.upsert))
      };

      return content;
    }

    /// <summary>
    /// Takes in a boolean and returns an all lower-case "yes" for true or "no" for false.
    /// </summary>
    /// <param name="flag">the boolean input value.</param>
    /// <returns>a string representing true or false as yes or no.</returns>
    private string boolToYesNo(bool flag)
    {
      return (flag) ? "yes" : "no";
    }
  }
}
