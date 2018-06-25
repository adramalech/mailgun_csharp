using System;
using System.Net.Mail;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Supression
{
  public interface IComplaintRequest
  {
    MailAddress Address { get; }
    DateTime? CreatedAt { get; }

    JObject ToJson();
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}
