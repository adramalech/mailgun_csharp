using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Net.Mail;
using Newtonsoft.Json.Linq;
using MailgunSharp.Enums;
using MailgunSharp.Wrappers;
using NodaTime;

namespace MailgunSharp.Supression
{
  public sealed class BounceRequest : IBounceRequest
  {
    private readonly MailAddress emailAddress;

    /// <summary>
    /// A valid email address.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    public MailAddress EmailAddress => this.emailAddress;

    private readonly SmtpErrorCode code;

    /// <summary>
    /// SMTP Error code.
    /// </summary>
    /// <value>SmtpErrorCode type.</value>
    public SmtpErrorCode Code => this.code;

    private readonly string error;

    /// <summary>
    /// Error description.
    /// </summary>
    /// <value>string</value>
    public string Error => this.error;

    private readonly DateTime createdAt;

    /// <summary>
    /// Timestamp of a bounce event.
    /// </summary>
    /// <value>DateTime</value>
    public DateTime CreatedAt => this.createdAt;

    /// <summary>
    /// Create an instance of the bounce request class.
    /// </summary>
    /// <param name="emailAddress">The valid email address.</param>
    /// <param name="statusCode">The STMP Error status code.  Defaults to 550, Mailbox Unavailable.</param>
    /// <param name="error">The error description. Defaults to empty string.</param>
    /// <param name="createdAt">Timestamp of the bounced event. Defaults to current time UTC.</param>
    /// <param name="tzInfo">The optional timezone information for specific timezone awareness in the date.</param>
    public BounceRequest(MailAddress emailAddress, SmtpErrorCode statusCode = SmtpErrorCode.MAILBOX_UNAVAILABLE, string error = "", DateTime? createdAt = null, TimeZoneInfo tzInfo = null)
    {
      if (emailAddress == null)
      {
        throw new ArgumentNullException("Address cannot be null or emtpy!");
      }

      this.emailAddress = emailAddress;
      this.code = statusCode;
      this.error = error;

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
    /// Get the Bounce request object as json to be used in an http request.
    /// </summary>
    /// <returns>Json object</returns>
    public JObject ToJson()
    {
      var jsonObject = new JObject()
      {
        ["address"] = this.emailAddress.Address,
        ["code"] = ((int) this.code).ToString(),
        ["error"] = this.error,
        ["created_at"] = (((DateTimeOffset) DateTime.UtcNow).ToUnixTimeSeconds()).ToString()
      };

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
        new KeyValuePair<string, string>("address", this.emailAddress.ToString()),
        new KeyValuePair<string, string>("code", ((int)this.code).ToString()),
        new KeyValuePair<string, string>("error", this.error),
        new KeyValuePair<string, string>("created_at", (((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds()).ToString())
      };

      return content;
    }
  }
}
