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
    public MailAddress Address
    {
      get
      {
        return address;
      }
    }

    private readonly JObject variables;
    public JObject Variables
    {
      get
      {
        return variables;
      }
    }

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
