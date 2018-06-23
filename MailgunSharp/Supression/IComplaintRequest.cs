using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Supression
{
  public interface IComplaintRequest
  {
    string Address { get; }
    DateTime? CreatedAt { get; }

    JObject ToJson();
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}
