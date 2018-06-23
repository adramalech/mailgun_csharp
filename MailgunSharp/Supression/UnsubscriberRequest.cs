using System;
using System.Net.Mail;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MailgunSharp.Supression
{
  public sealed class UnsubscriberRequest : IUnsubscriberRequest
  {
    private string address;
    public string Address
    {
      get
      {
        return address;
      }
    }

    private string tag;
    public string Tag
    {
      get
      {
        return tag;
      }
    }

    private DateTime? createdAt;
    public DateTime? CreatedAt
    {
      get
      {
        return createdAt;
      }
    }

    public UnsubscriberRequest(string address, string tag = "*", DateTime? createdAt = null)
    {
      var emailAddress = new MailAddress(address);

      this.address = address;
      this.tag = tag;
      this.createdAt = createdAt;
    }

    public JObject ToJson()
    {
      var jsonObject = new JObject();

      jsonObject["address"] = this.address;
      jsonObject["tag"] = this.tag;
      jsonObject["created_at"] = ((!this.createdAt.HasValue) ? ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() : ((DateTimeOffset)this.createdAt.Value).ToUnixTimeSeconds()).ToString();

      return jsonObject;
    }

    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("address", this.address),
        new KeyValuePair<string, string>("tag", this.tag),
        new KeyValuePair<string, string>("created_at", ((!this.createdAt.HasValue) ? ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() : ((DateTimeOffset)this.createdAt.Value).ToUnixTimeSeconds()).ToString())
      };

      return content;
    }
  }
}
