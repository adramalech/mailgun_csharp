using System;
using System.Net.Mail;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Messages
{
  public sealed class Recipient : IRecipient
  {
    private readonly MailAddress address;

    /// <summary>
    /// A valid email address of a recipient to send a message to.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    public MailAddress Address => address;

    private readonly JObject variables;

    /// <summary>
    /// A set of variables to be referenced in the message template.
    /// </summary>
    /// <value>Json object</value>
    public JObject Variables => variables;

    /// <summary>
    /// Create an instance of Recipient class.
    /// </summary>
    /// <param name="address">A valid email address.</param>
    /// <param name="variables">Recipient variables will be referenced in message template.</param>
    /// <exception cref="ArgumentNullException">Address cannot be null or empty.</exception>
    public Recipient(MailAddress address, JObject variables = null)
    {
      this.address = address ?? throw new ArgumentNullException(nameof(address), "Address cannot be null or empty!");
      this.variables = variables;
    }
  }
}
