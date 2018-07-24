using System;
using System.Net.Mail;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MailgunSharp.Supression
{
  public sealed class UnsubscriberRequest : IUnsubscriberRequest
  {
    private readonly MailAddress address;

    /// <summary>
    /// A valid email address.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    public MailAddress Address
    {
      get
      {
        return this.address;
      }
    }

    private readonly string tag;

    /// <summary>
    /// Unsubscribe from a specific tag.
    ///
    /// Use "*", to unsubscribe an address from all domain's correspondence.
    /// </summary>
    /// <value>string</value>
    public string Tag
    {
      get
      {
        return this.tag;
      }
    }

    private readonly DateTime createdAt;

    /// <summary>
    /// Timestamp of an unsubscribe event.
    /// </summary>
    /// <value>DateTime</value>
    public DateTime CreatedAt
    {
      get
      {
        return this.createdAt;
      }
    }

    /// <summary>
    /// Create an instance of unsubscriber request class.
    /// </summary>
    /// <param name="address">A valid email address.</param>
    /// <param name="tag">A specific tag to unsubscribe from, will default to "*" which will unsubscribe an address from all domain's correspondence.</param>
    /// <param name="createdAt">Timestamp of an unsubscribe event.</param>
    /// <param name="tzInfo">The optional timezone information for specific timezone awareness in the date.</param>
    public UnsubscriberRequest(MailAddress address, string tag = "*", DateTime? createdAt = null, TimeZoneInfo tzInfo = null)
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      this.address = address;
      this.tag = tag;

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
    /// Get Unsubscriber request object represented as a json object for http request.
    /// </summary>
    /// <returns>Json object</returns>
    public JObject ToJson()
    {
      var jsonObject = new JObject
      {
        ["address"] = this.address.Address,
        ["tag"] = this.tag,
        ["created_at"] = (((DateTimeOffset) DateTime.UtcNow).ToUnixTimeSeconds()).ToString()
      };

      return jsonObject;
    }

    /// <summary>
    /// Get Unsubscriber request object represented as a key-value string pair form content for http request.
    /// </summary>
    /// <returns>List key-value string pairs</returns>
    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("address", this.address.Address),
        new KeyValuePair<string, string>("tag", this.tag),
        new KeyValuePair<string, string>("created_at", (((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds()).ToString())
      };

      return content;
    }
  }
}
