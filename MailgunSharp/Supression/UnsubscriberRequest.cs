using System;
using System.Net.Mail;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MailgunSharp.Extensions;
using NodaTime;

namespace MailgunSharp.Supression
{
  public sealed class UnsubscriberRequest : IUnsubscriberRequest
  {
    private readonly MailAddress address;

    /// <summary>
    /// A valid email address.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    public MailAddress Address => this.address;

    private readonly string tag;

    /// <summary>
    /// Unsubscribe from a specific tag.
    ///
    /// Use "*", to unsubscribe an address from all domain's correspondence.
    /// </summary>
    /// <value>string</value>
    public string Tag => this.tag;

    private readonly Instant? createdAt;

    /// <summary>
    /// Timestamp of an unsubscribe event.
    /// </summary>
    /// <value>DateTime</value>
    public Instant? CreatedAt => this.createdAt;

    /// <summary>
    /// Create an instance of unsubscriber request class.
    /// </summary>
    /// <param name="address">A valid email address.</param>
    /// <param name="tag">A specific tag to unsubscribe from, will default to "*" which will unsubscribe an address from all domain's correspondence.</param>
    /// <param name="createdAt">Timestamp of an unsubscribe event.</param>
    /// <exception cref="ArgumentNullException">The address is a required parameter.</exception>
    public UnsubscriberRequest(MailAddress address, string tag = "*", Instant? createdAt = null)
    {
      this.address = address ?? throw new ArgumentNullException(nameof(address), "Address cannot be null or empty!");
      this.tag = tag;
      this.createdAt = (createdAt.HasValue) ? this.createdAt : null;
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
        ["tag"] = this.tag
      };

      if (this.createdAt.HasValue)
      {
        jsonObject.Add("created_at", this.createdAt.Value.ToRfc2822DateFormat());
      }

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
        new KeyValuePair<string, string>("tag", this.tag)
      };

      if (this.createdAt.HasValue)
      {
        content.Add("created_at", this.createdAt.Value.ToRfc2822DateFormat());
      }

      return content;
    }
  }
}
