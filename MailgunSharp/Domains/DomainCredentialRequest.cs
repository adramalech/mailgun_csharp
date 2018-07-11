using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Domains
{
  public sealed class DomainCredentialRequest : IDomainCredentialRequest
  {
    /// <summary>
    /// The username for the smtp domain credentials.
    /// </summary>
    /// <value>string</value>
    private readonly string username;
    public string Username
    {
      get
      {
        return username;
      }
    }

    /// <summary>
    /// The password for the smtp domain credentials.
    /// </summary>
    /// <value>string</value>
    private readonly string password;
    public string Password
    {
      get
      {
        return password;
      }
    }

    /// <summary>
    /// Create an instance of the domain credential.
    /// </summary>
    /// <param name="username">The username as a string.</param>
    /// <param name="password">The smtp password of a string. Must be a minimum length of 5 and maximum length of 32.</param>
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

    /// <summary>
    /// Get the Domain Credential as a json object.
    /// </summary>
    /// <returns>A Json object of the Domain Credential request object.</returns>
    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("login", username),
        new KeyValuePair<string, string>("password", password)
      };

      return content;
    }

    /// <summary>
    /// Get the Domain Credential as a form content key-value pair strings.
    /// </summary>
    /// <returns>List of keyvalue string pairs.</returns>
    public JObject ToJson()
    {
      var jsonObject = new JObject();

      jsonObject["login"] = this.username;
      jsonObject["password"] = this.password;

      return jsonObject;
    }

    /// <summary>
    /// Check the characteristics of the password.
    /// </summary>
    /// <param name="password">The password string.</param>
    /// <returns>True if the password meets the minumum length requirement of 5, and a maximum length of 32.</returns>
    private bool checkPasswordLengthRequirement(string password)
    {
      var length = password.Length;

      return (length > 4 && length < 33);
    }

    /// <summary>
    /// Check if the string only is null, empty, or whitespace.
    /// </summary>
    /// <param name="str">The string to check.</param>
    /// <returns>True, if the string is only null, empty, or whitespace; false, if it isn't null, empty, or whitespace.</returns>
    private bool checkStringIfNullEmptyWhitespace(string str)
    {
      return (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
    }
  }
}