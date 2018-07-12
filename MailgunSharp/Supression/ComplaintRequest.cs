using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Supression
{
  public sealed class ComplaintRequest : IComplaintRequest
  {
    /// <summary>
    /// Valid email address.
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
    /// Timestamp of a complaint event.
    /// </summary>
    /// <value>DateTime</value>
    private readonly DateTime? createdAt;
    public DateTime? CreatedAt
    {
      get
      {
        return createdAt;
      }
    }

    /// <summary>
    /// Create an instance of complaint request class.
    /// </summary>
    /// <param name="address">A valid email address.</param>
    /// <param name="createdAt">Timestamp as datetime UTC.</param>
    public ComplaintRequest(MailAddress address, DateTime? createdAt = null)
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      this.address = address;
      this.createdAt = createdAt;
    }

    /// <summary>
    /// Get Complaint Request object represented as json object for http request.
    /// </summary>
    /// <returns>Json object</returns>
    public JObject ToJson()
    {
      var jsonObject = new JObject();

      jsonObject["address"] = this.address.Address;
      jsonObject["created_at"] = ((!this.createdAt.HasValue) ? ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() : ((DateTimeOffset)this.createdAt.Value).ToUnixTimeSeconds()).ToString();

      return jsonObject;
    }

    /// <summary>
    /// Get Complaint Request object represented as key-value string pair form content for http request.
    /// </summary>
    /// <returns></returns>
    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("address", this.address.Address),
        new KeyValuePair<string, string>("created_at", ((!this.createdAt.HasValue) ? ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() : ((DateTimeOffset)this.createdAt.Value).ToUnixTimeSeconds()).ToString())
      };

      return content;
    }
  }
}
