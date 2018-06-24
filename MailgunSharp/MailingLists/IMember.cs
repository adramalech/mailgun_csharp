using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.MailingLists
{
  public interface IMember
  {
    MailAddress Address { get; }
    string Name { get; }
    JObject Vars { get; }
    bool Subscribed { get; }
    bool Upsert { get; }

    JObject ToJson();
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}
