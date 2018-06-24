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

    private readonly JObject vars;
    public JObject Vars
    {
      get
      {
        return vars;
      }
    }

    private readonly bool subscribed;
    public bool Subscribed
    {
      get
      {
        return subscribed;
      }
    }

    private readonly bool upsert;
    public bool Upsert
    {
      get
      {
        return upsert;
      }
    }

    public Member(MailAddress address, string name = "", JObject vars = null,  bool subscribed = false, bool upsert = false)
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

    private string boolToYesNo(bool flag)
    {
      return (flag) ? "yes" : "no";
    }
  }
}
