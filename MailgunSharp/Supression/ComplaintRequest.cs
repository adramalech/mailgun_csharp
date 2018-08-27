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

    /// <summary>
    /// Valid email address.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    public MailAddress Address => this.address;

    private readonly DateTime createdAt;

    /// <summary>
    /// Timestamp of a complaint event.
    /// </summary>
    /// <value>DateTime</value>
    public DateTime CreatedAt => this.createdAt;

    /// <summary>
    /// Create an instance of complaint request class.
    /// </summary>
    /// <param name="address">A valid email address.</param>
    /// <param name="createdAt">Timestamp as datetime UTC.</param>
    /// <param name="tzInfo">The optional timezone information for specific timezone awareness in the date.</param>
    public ComplaintRequest(MailAddress address, DateTime? createdAt = null, TimeZoneInfo tzInfo = null)
    {
      if (address == null)
      {
        throw new ArgumentNullException(nameof(address), "Address cannot be null or empty!");
      }

      this.address = address;

      if (createdAt.HasValue)
      {
        this.createdAt = (tzInfo == null) ? createdAt.Value.ToUniversalTime() : TimeZoneInfo.ConvertTimeToUtc(createdAt.Value.ToUniversalTime(), tzInfo);
      }
      else
      {
        this.createdAt = (tzInfo == null) ? DateTime.UtcNow : TimeZoneInfo.ConvertTimeToUtc(DateTime.UtcNow, tzInfo);
      }
    }

    /// <summary>
    /// Get Complaint Request object represented as json object for http request.
    /// </summary>
    /// <returns>Json object</returns>
    public JObject ToJson()
    {
      var jsonObject = new JObject
      {
        ["address"] = this.address.Address,
        ["created_at"] = (((DateTimeOffset) this.createdAt).ToUnixTimeSeconds()).ToString()
      };

      return jsonObject;
    }

    /// <summary>
    /// Get Complaint Request object represented as key-value string pair form content for http request.
    /// </summary>
    /// <returns>List of key-value string pairs.</returns>
    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("address", this.address.Address),
        new KeyValuePair<string, string>("created_at", (((DateTimeOffset)this.createdAt).ToUnixTimeSeconds()).ToString())
      };

      return content;
    }
  }
}
