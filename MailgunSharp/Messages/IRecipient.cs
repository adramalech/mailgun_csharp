using System.Net.Mail;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Messages
{
  public interface IRecipient
  {
    /// <summary>
    /// A valid email address of a recipient to send a message to.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    MailAddress Address { get; }

    /// <summary>
    /// A set of variables to be referenced in the message body.
    /// </summary>
    /// <value>Json object</value>
    JObject Variables { get; }
  }
}
