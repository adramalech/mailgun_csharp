using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Supression
{
  public interface IBounceRequest
  {
    string Address { get; }
    SmtpErrorCode Code { get; }
    string Error { get; }
    DateTime? CreatedAt { get; }

    JObject ToJson();
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}
