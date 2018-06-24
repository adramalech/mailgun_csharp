using System;
using System.Text;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MailgunSharp.MailingLists
{
  public interface IMailingList
  {
    MailAddress Address { get; }
    string Name { get; }
    string Description { get; }
    AccessLevel AccessLevel { get; }

    JObject ToJson();
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}
