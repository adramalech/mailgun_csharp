using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using MailgunSharp.Enums;

namespace MailgunSharp.Domains
{
  public interface IDomainRequest
  {
    Uri Name { get; }
    NetworkCredential SmtpPassword { get; }
    SpamAction SpamAction { get; }
    bool Wildcard { get; }
    bool ForceDKIMAuthority { get; }

    JObject ToJson();
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}