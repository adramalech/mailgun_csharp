using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Net.Mail;
using Newtonsoft.Json.Linq;
using MailgunSharp.Enums;
using MailgunSharp.Extensions;
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

    private readonly Instant? createdAt;

    /// <summary>
    /// Timestamp of a bounce event.
    /// </summary>
    /// <value>DateTime</value>
    public Instant? CreatedAt => this.createdAt;

    /// <summary>
    /// Create an instance of the bounce request class.
    /// </summary>
    /// <param name="emailAddress">The valid email address.</param>
    /// <param name="code">The STMP Error status code.  Defaults to 550, Mailbox Unavailable.</param>
    /// <param name="error">The error description. Defaults to empty string.</param>
    /// <param name="createdAt">Timestamp of the bounced event. Defaults to current time UTC.</param>
    /// <exception cref="ArgumentNullException">The email address is a required parameter.</exception>
    public BounceRequest(MailAddress emailAddress, SmtpErrorCode code = SmtpErrorCode.MAILBOX_UNAVAILABLE, string error = "", Instant? createdAt = null)
    {
      this.emailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress), "Address cannot be null or empty!");
      this.code = code;
      this.error = error;

      if (createdAt.HasValue)
      {
        this.createdAt = createdAt.Value;
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
        ["error"] = this.error
      };

      //add the optional created_at datetime value if provided.
      if (this.createdAt.HasValue)
      {
        jsonObject.Add("created_at", this.createdAt.Value.ToRfc2822DateFormat());
      }

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
        new KeyValuePair<string, string>("error", this.error)
      };

      //add the optional created_at datetime value if provided.
      if (this.createdAt.HasValue)
      {
        content.Add(new KeyValuePair<string, string>("created_at", this.createdAt.Value.ToRfc2822DateFormat()));
      }

      return content;
    }
  }
}
