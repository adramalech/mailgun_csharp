using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MailgunSharp.Extensions;
using Newtonsoft.Json.Linq;
using NodaTime;

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

    private readonly Instant? createdAt;

    /// <summary>
    /// Timestamp of a complaint event.
    /// </summary>
    /// <value>NodaTime.Instant</value>
    public Instant? CreatedAt => this.createdAt;

    /// <summary>
    /// Create an instance of complaint request class.
    /// </summary>
    /// <param name="address">A valid email address.</param>
    /// <param name="createdAt">Timestamp as datetime UTC.</param>
    /// <exception cref="ArgumentNullException">The address is a required parameter.</exception>
    public ComplaintRequest(MailAddress address, Instant? createdAt = null)
    {
      this.address = address ?? throw new ArgumentNullException(nameof(address), "Address cannot be null or empty!");
      this.createdAt = (createdAt.HasValue) ? createdAt : null;
    }

    /// <summary>
    /// Get Complaint Request object represented as json object for http request.
    /// </summary>
    /// <returns>Json object</returns>
    public JObject ToJson()
    {
      var jsonObject = new JObject
      {
        ["address"] = this.address.Address
      };

      if (this.createdAt.HasValue)
      {
        jsonObject.Add("created_at", this.createdAt.Value.ToRfc2822DateFormat());
      }

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
        new KeyValuePair<string, string>("address", this.address.Address)
      };

      if (this.createdAt.HasValue)
      {
        content.Add("created_at", this.createdAt.Value.ToRfc2822DateFormat());
      }

      return content;
    }
  }
}
