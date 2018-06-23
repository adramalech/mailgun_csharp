using System.Net.Mail;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Messages
{
  public interface IRecipient
  {
    MailAddress Address { get; }
    JObject Variables { get; }
  }
}
