using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    public MailAddress Address
    {
      get
      {
        return address;
      }
    }

    private readonly JObject variables;

    /// <summary>
    /// A set of variables to be referenced in the message template.
    /// </summary>
    /// <value>Json object</value>
    public JObject Variables
    {
      get
      {
        return variables;
      }
    }

    /// <summary>
    /// Create an instance of Recipient class.
    /// </summary>
    /// <param name="address">A valid email address.</param>
    /// <param name="variables">Recipient variables will be referenced in message template.</param>
    public Recipient(MailAddress address, JObject variables = null)
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      this.address = address;
      this.variables = variables;
    }
  }
}
