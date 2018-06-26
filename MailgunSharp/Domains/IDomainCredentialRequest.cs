using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Domains
{
    public interface IDomainCredentialRequest
    {
       string Username { get; }
       string Password { get; }

       JObject ToJson();
       ICollection<KeyValuePair<string, string>> ToFormContent();
    }
}