using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using MailgunSharp.Extensions;

namespace MailgunSharp.Domains
{
  public sealed class DomainCredentialRequest : IDomainCredentialRequest
  {
    /// <summary>
    /// Maximum allowed SMTP password length.
    /// </summary>
    private const int MAX_SMTP_PASSWORD_LENGTH = 32;

    /// <summary>
    /// Minimum allowed SMTP password length.
    /// </summary>
    private const int MIN_SMTP_PASSWORD_LENGTH = 5;

    private readonly string username;

    /// <summary>
    /// The username for the smtp domain credentials.
    /// </summary>
    /// <value>string</value>
    public string Username => this.username;

    private readonly string password;

    /// <summary>
    /// The password for the smtp domain credentials.
    /// </summary>
    /// <value>string</value>
    public string Password => this.password;

    /// <summary>
    /// Create an instance of the domain credential.
    /// </summary>
    /// <param name="username">The username as a string.</param>
    /// <param name="password">The smtp password of a string. Must be a minimum length of 5 and maximum length of 32.</param>
    /// <exception cref="ArgumentOutOfRangeException">Password has a required minimum length of 5 and a maximum length of 32.</exception>
    /// <exception cref="ArgumentNullException">Domain Credential username and password are required and cannot be null, empty, or whitespace.</exception>
    public DomainCredentialRequest(string username, string password)
    {
      if (username.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(username), "Username cannot be null or empty!");
      }

      if (password.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(password), "Password cannot be null or empty!");
      }

      if (!this.isPasswordLengthWithinRequiredRange(password))
      {
        throw new ArgumentOutOfRangeException(nameof(password), "Password must have a minimum length of 5, and maximum length of 32!");
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
    /// <returns>List of key-value string pairs.</returns>
    public JObject ToJson()
    {
      var jsonObject = new JObject
      {
        ["login"] = this.username,
        ["password"] = this.password
      };

      return jsonObject;
    }

    /// <summary>
    /// Check the characteristics of the password.
    /// </summary>
    /// <param name="password">The password string.</param>
    /// <returns>True if the password meets the minimum length requirement of 5, and a maximum length of 32.</returns>
    private bool isPasswordLengthWithinRequiredRange(string password)
    {
      var length = password.Length;

      return (length >= MIN_SMTP_PASSWORD_LENGTH && length <= MAX_SMTP_PASSWORD_LENGTH);
    }
  }
}
