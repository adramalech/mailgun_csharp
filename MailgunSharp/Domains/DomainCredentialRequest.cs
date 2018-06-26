using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Domains
{
  public sealed class DomainCredentialRequest : IDomainCredentialRequest
  {
    private readonly string username;  
    public string Username 
    { 
      get 
      { 
        return username; 
      } 
    }

    private readonly string password;
    public string Password 
    { 
      get 
      { 
        return password; 
      } 
    }

    public DomainCredentialRequest(string username, string password)
    {
      if (checkStringIfNullEmptyWhitespace(username))
      {
        throw new ArgumentNullException("Username cannot be null or empty!");
      }

      if (checkStringIfNullEmptyWhitespace(password))
      {
        throw new ArgumentNullException("Password cannot be null or empty!");
      }

      var length = password.Length;


      if (checkPasswordLengthRequirement(password))
      {
        throw new ArgumentOutOfRangeException("Password must have a minimum length of 5, and maximum length of 32!");
      }

      this.username = username;
      this.password = password;
    }

    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("login", username),
        new KeyValuePair<string, string>("password", password)
      };

      return content;
    }

    public JObject ToJson()
    {
      var jsonObject = new JObject();

      jsonObject["login"] = this.username;
      jsonObject["password"] = this.password;

      return jsonObject;
    }

    private bool checkPasswordLengthRequirement(string password)
    {
      var length = password.Length;

      return (length > 4 && length < 33);
    }

    private bool checkStringIfNullEmptyWhitespace(string str)
    {
      return (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
    }
  }
}