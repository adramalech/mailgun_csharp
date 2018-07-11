using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Net.Mail;
using Newtonsoft.Json.Linq;
using MailgunSharp.Enums;

namespace MailgunSharp.Supression
{
  public sealed class BounceRequest : IBounceRequest
  {
    /// <summary>
    /// A valid email address.
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
    /// SMTP Error code.
    /// </summary>
    /// <value>SmtpErrorCode type.</value>
    private readonly SmtpErrorCode code;
    public SmtpErrorCode Code
    {
      get
      {
        return code;
      }
    }

    /// <summary>
    /// Error description.
    /// </summary>
    /// <value>string</value>
    private readonly string error;
    public string Error
    {
      get
      {
        return error;
      }
    }

    /// <summary>
    /// Timestamp of a bounce event.
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
    /// Create an instance of the bounce request class.
    /// </summary>
    /// <param name="address">The valid email address.</param>
    /// <param name="statusCode">The STMP Error status code.  Defaults to 550, Mailbox Unavailable.</param>
    /// <param name="error">The error description. Defaults to empty string.</param>
    /// <param name="createdAt">Timestamp of the bounced event. Defaults to current time UTC.</param>
    public BounceRequest(MailAddress address, SmtpErrorCode statusCode = SmtpErrorCode.MAILBOX_UNAVAILABLE, string error = "", DateTime? createdAt = null)
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or emtpy!");
      }

      this.address = address;
      this.code = statusCode;
      this.error = error;
      this.createdAt = createdAt;
    }

    /// <summary>
    /// Get the Bounce request object as json to be used in an http request.
    /// </summary>
    /// <returns>Json object</returns>
    public JObject ToJson()
    {
      var jsonObject = new JObject();

      jsonObject["address"] = this.address.Address;
      jsonObject["code"] = ((int)this.code).ToString();
      jsonObject["error"] = this.error;
      jsonObject["created_at"] = ((!this.createdAt.HasValue) ? ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() : ((DateTimeOffset)this.createdAt.Value).ToUnixTimeSeconds()).ToString();

      return jsonObject;
    }

    /// <summary>
    /// Get the Bounce request object as form content list key-value string pair to be used in an http request.
    /// </summary>
    /// <returns>List key-value string pair</returns>
    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("address", this.address.ToString()),
        new KeyValuePair<string, string>("code", ((int)this.code).ToString()),
        new KeyValuePair<string, string>("error", this.error),
        new KeyValuePair<string, string>("created_at", ((!this.createdAt.HasValue) ? ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() : ((DateTimeOffset)this.createdAt.Value).ToUnixTimeSeconds()).ToString())
      };

      return content;
    }
  }
}
