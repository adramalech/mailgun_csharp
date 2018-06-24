using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Net.Mail;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Supression
{
  public sealed class BounceRequest : IBounceRequest
  {
    private readonly string address;
    public string Address
    {
      get
      {
        return address;
      }
    }

    private readonly SmtpErrorCode code;
    public SmtpErrorCode Code
    {
      get
      {
        return code;
      }
    }

    private readonly string error;
    public string Error
    {
      get
      {
        return error;
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

    public BounceRequest(string address, SmtpErrorCode statusCode = SmtpErrorCode.MAILBOX_UNAVAILABLE, string error = "", DateTime? createdAt = null)
    {
      var emailAddress = new MailAddress(address);

      this.address = address;
      this.code = statusCode;
      this.error = error;
      this.createdAt = createdAt;
    }

    public JObject ToJson()
    {
      var jsonObject = new JObject();

      jsonObject["address"] = this.address;
      jsonObject["code"] = ((int)this.code).ToString();
      jsonObject["error"] = this.error;
      jsonObject["created_at"] = ((!this.createdAt.HasValue) ? ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() : ((DateTimeOffset)this.createdAt.Value).ToUnixTimeSeconds()).ToString();

      return jsonObject;
    }

    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("address", this.address),
        new KeyValuePair<string, string>("code", ((int)this.code).ToString()),
        new KeyValuePair<string, string>("error", this.error),
        new KeyValuePair<string, string>("created_at", ((!this.createdAt.HasValue) ? ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() : ((DateTimeOffset)this.createdAt.Value).ToUnixTimeSeconds()).ToString())
      };

      return content;
    }
  }
}
