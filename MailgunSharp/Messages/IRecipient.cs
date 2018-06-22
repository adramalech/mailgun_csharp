using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Messages
{
  public interface IRecipient
  {
    string Name { get; }
    string Address { get; }
    JObject Variables { get; }
    string ToFormattedNameAddress();
  }
}
