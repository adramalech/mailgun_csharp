using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MailgunSharp.Supression
{
  public interface IUnsubscriberRequest
  {
    string Address { get; }
    string Tag { get; }
    DateTime? CreatedAt { get; }

    JObject ToJson();
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}
