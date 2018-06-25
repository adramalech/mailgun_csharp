using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Supression
{
  public sealed class ComplaintRequest : IComplaintRequest
  {
    private readonly MailAddress address;
    public MailAddress Address
    {
      get
      {
        return address;
      }
    }

    private readonly DateTime? createdAt;
    public DateTime? CreatedAt
    {
      get
      {
        return createdAt;
      }
    }

    public ComplaintRequest(MailAddress address, DateTime? createdAt = null)
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      this.address = address;
      this.createdAt = createdAt;
    }

    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("address", this.address.Address),
        new KeyValuePair<string, string>("created_at", ((!this.createdAt.HasValue) ? ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() : ((DateTimeOffset)this.createdAt.Value).ToUnixTimeSeconds()).ToString())
      };

      return content;
    }

    public JObject ToJson()
    {
      var jsonObject = new JObject();

      jsonObject["address"] = this.address.Address;
      jsonObject["created_at"] = ((!this.createdAt.HasValue) ? ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() : ((DateTimeOffset)this.createdAt.Value).ToUnixTimeSeconds()).ToString();

      return jsonObject;
    }
  }
}
