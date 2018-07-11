using System;
using System.Net.Mail;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using MailgunSharp.Enums;

namespace MailgunSharp.Supression
{
  public interface IBounceRequest
  {
    MailAddress Address { get; }
    SmtpErrorCode Code { get; }
    string Error { get; }
    DateTime? CreatedAt { get; }

    JObject ToJson();
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}
