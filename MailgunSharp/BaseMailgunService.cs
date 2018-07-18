using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using MailgunSharp.Extensions;
using MailgunSharp.MailingLists;
using MailgunSharp.Domains;
using MailgunSharp.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MailgunSharp
{
  public class BaseMailgunService : IBaseMailgunService
  {
    /// <summary>
    /// The base url for the mailgun v3 api.
    /// </summary>
    protected const string MAILGUN_BASE_URL = @"https://api.mailgun.net/v3/";

    /// <summary>
    /// Maximum allowed SMTP password length.
    /// </summary>
    protected const int MAX_SMTP_PASSWORD_LENGTH = 32;

    /// <summary>
    /// Minimum allowed SMTP password length.
    /// </summary>
    protected const int MIN_SMTP_PASSWORD_LENGTH = 5;

    /// <summary>
    /// The maximum number of json objects that can be added in a request.
    /// </summary>
    protected const int MAX_JSON_OBJECTS = 1000;

    /// <summary>
    /// The maximum number of records that can be returned in a response of a supression api request.
    /// </summary>
    protected const int MAX_RECORD_LIMIT = 10000;

    /// <summary>
    /// The maximum string length of a list of email addresses to be used in a email address parse validation request.
    /// </summary>
    protected const int MAX_ADDRESS_LENGTH = 8000;

    /// <summary>
    /// The http client instance to be used to make http requests to mailgun's api.
    /// </summary>
    protected readonly HttpClient httpClient;

    public BaseMailgunService(string apiKey, HttpClient httpClient = null)
    {
      if (apiKey.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Api key cannot be null!");
      }

      this.httpClient = (httpClient == null) ? new HttpClient() : httpClient;

      this.httpClient.BaseAddress = new Uri(MAILGUN_BASE_URL);

      this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{apiKey}")));
    }

    /// <summary>
    /// Given a arbitrary email address, validates address based off defined checks.
    ///
    /// Uses the private api call, using the private auth api key, and will not be subject to rate limits of the public api calls.
    /// </summary>
    /// <param name="address">The email address to be validated.</param>
    /// <param name="validateMailbox">True, a mailbox verification check will be performed against the address, false will not.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> ValidateEmailAddressAsync(MailAddress address, bool validateMailbox = false, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"address/private/validate?address={address.Address}&mailbox_verification={validateMailbox.ToString()}", ct);
    }

    /// <summary>
    /// Parses a single email address. Will determine if the address is syntactically valid, and optionally pass DNS and ESP specific grammar checks.
    ///
    /// Uses the private api call, using the private auth api key, and will not be subject to rate limits of the public api calls.
    /// </summary>
    /// <param name="address">The email address to check.</param>
    /// <param name="syntaxOnly"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> ParseEmailAddressAsync(string address, bool syntaxOnly = true, CancellationToken ct = default(CancellationToken))
    {
      if (address.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Recipients cannot be null or empty!");
      }

      return parseAddressesAsync(address, syntaxOnly, ct);
    }

    /// <summary>
    /// Parses a list of email addresses. Will determine if the addresses are syntactically valid, and optionally pass DNs and ESP specific grammar checks.
    ///
    /// Uses the private api call, using the private auth api key, and will not be subject to rate limits of the public api calls.
    /// </summary>
    /// <param name="addresses">The list of addresses to check.</param>
    /// <param name="syntaxOnly">True, perform only syntax checks; false, DNS and ESP specific validation as well.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> ParseEmailAddressesAsync(ICollection<string> addresses, bool syntaxOnly = true, CancellationToken ct = default(CancellationToken))
    {
      if (addresses == null || addresses.Count < 1)
      {
        throw new ArgumentNullException("Recipients cannot be null or empty!");
      }

      var addressList = addresses.ToString();

      return parseAddressesAsync(addressList, syntaxOnly, ct);
    }

    /// <summary>
    /// Make the request to parse a list of email addresses that are comma delimited.
    ///
    /// Maximum allowed character of email address list of 8,000 characters.
    /// </summary>
    /// <param name="addressList">List of email addresses to be parsed.</param>
    /// <param name="syntaxOnly">Perform only syntax checks or DNS and ESP specific validation as well.</param>
    /// <param name="ct">The async cancellation token.</param>
    /// <returns></returns>
    protected Task<HttpResponseMessage> parseAddressesAsync(string addressList, bool syntaxOnly, CancellationToken ct)
    {
      if (addressList.Length > MAX_ADDRESS_LENGTH)
      {
        throw new ArgumentOutOfRangeException("List of email addresses to parse cannot exceed a maximum length of 8,000 characters!");
      }

      return this.httpClient.GetAsync($"address/private/validate?addresses={addressList}&syntax_only={syntaxOnly.ToString()}", ct);
    }

    /// <summary>
    /// Returns a list of IPs assigned to your account.
    /// </summary>
    /// <param name="onlyShowDedicated">True, return only dedicated IPs; false, return all IPs.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetIPsAsync(bool onlyShowDedicated = false, CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.GetAsync($"ips?dedicated={onlyShowDedicated.ToString()}", ct);
    }

    /// <summary>
    /// Return information about the specified IP address.
    /// </summary>
    /// <param name="ipV4Address">The correctly formatted IP v4 address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetIPDetailsAsync(string ipV4Address, CancellationToken ct = default(CancellationToken))
    {
      if (!isIPv4AddressValid(ipV4Address))
      {
        throw new FormatException("IP V4 Address is incorrectly formatted, must be in the format ###.###.###.###!");
      }

      return this.httpClient.GetAsync($"ips/{ipV4Address}", ct);
    }

    /// <summary>
    /// Check if the IP Address is a valid IP v4 formatted address in the range of 0.0.0.0 to 255.255.255.255.
    /// </summary>
    /// <param name="ipV4Address">The ip v4 address to check.</param>
    /// <returns>True, if the ip address is valid, false if the ip address is invalid.</returns>
    protected bool isIPv4AddressValid(string ipV4Address)
    {
      return Regex.IsMatch(ipV4Address, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
    }

    /// <summary>
    /// Paginate over mailing lists under your account.
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetMailingLists(int limit = 100, CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.GetAsync($"lists/pages?limit={limit}", ct);
    }

    /// <summary>
    /// Returns a single mailing list by a given address.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetMailingList(MailAddress address, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.DeleteAsync($"lists/{address.Address}", ct);
    }

    /// <summary>
    /// Creates a new mailing list.
    /// </summary>
    /// <param name="mailingList">The mailing list to be added.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> AddMailingList(IMailingList mailingList, CancellationToken ct = default(CancellationToken))
    {
      if (mailingList == null)
      {
        throw new ArgumentNullException("Mailing List object cannot be null or empty!");
      }

      var formContent = new FormUrlEncodedContent(mailingList.ToFormContent());

      return this.httpClient.PostAsync("lists", formContent, ct);
    }

    /// <summary>
    /// Update mailnig list properties, such as address, description or name.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="mailingList">The mailing list to be updated.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> UpdateMailingList(MailAddress address, IMailingList mailingList, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      if (mailingList == null)
      {
        throw new ArgumentNullException("Mailing List object cannot be null or empty!");
      }

      var formContent = new FormUrlEncodedContent(mailingList.ToFormContent());

      return this.httpClient.PutAsync($"lists/{address.Address}", formContent, ct);
    }

    /// <summary>
    /// Deletes a mailing list.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> DeleteMailingList(MailAddress address, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.DeleteAsync($"lists/{address.Address}", ct);
    }

    /// <summary>
    /// Paginate over list of members in the given mailing list.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="subscribed">True, lists subscribed; false, for list unsubscribed. If null will list all.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetMailingListMembers(MailAddress address, int limit = 100, bool? subscribed = null, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      var subbed = (subscribed.HasValue) ? subscribed.Value.ToYesNo() : "";

      var url = (subbed.IsNullEmptyWhitespace()) ? $"lists/{address.Address}/members/pages?limit={limit}" : $"lists/{address.Address}/members/pages?limit={limit}&subscribed={subbed}";

      return this.httpClient.GetAsync(url, ct);
    }

    /// <summary>
    /// Retrieves a mailing list member.
    /// </summary>
    /// <param name="mailingListAddress">The mailing list's email address.</param>
    /// <param name="memberAddress">The member's email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetMailingListMember(MailAddress mailingListAddress, MailAddress memberAddress, CancellationToken ct = default(CancellationToken))
    {
      if (mailingListAddress == null)
      {
        throw new ArgumentNullException("Mailing List Address cannot be null or empty!");
      }

      if (memberAddress == null)
      {
        throw new ArgumentNullException("Member Address cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"lists/{mailingListAddress.Address}/members/{memberAddress.Address}", ct);
    }

    /// <summary>
    /// Adds a member to the mailing list.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="member">A mailing list member.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> AddMailingListMember(MailAddress address, IMember member, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      if (member == null)
      {
        throw new ArgumentNullException("Member object cannot be null or empty!");
      }

      var formContent = new FormUrlEncodedContent(member.ToFormContent());

      return this.httpClient.PostAsync($"lists/{address}/members", formContent, ct);
    }

    /// <summary>
    /// Adds multiple members, up to 1,000 per call, to a Mailing list.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="members">List of members to be added to the mailing list.</param>
    /// <param name="upsert">True, to update existing members; false, to ignore duplicates.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> AddMailingListMembers(MailAddress address, ICollection<IMember> members, bool upsert, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      if (members == null)
      {
        throw new ArgumentNullException("Members object cannot be null or empty!");
      }

      if (members.Count > MAX_JSON_OBJECTS)
      {
        throw new ArgumentNullException("Members object cannot exceed maximum limit of 1,000 records!");
      }

      var json = new JObject();

      var jsonArray = new JArray();

      foreach(var member in members)
      {
        jsonArray.Add(member.ToJson());
      }

      json["members"] = jsonArray.ToString(Formatting.None);
      json["upsert"] = upsert.ToYesNo();

      return this.httpClient.PostAsync($"lists/{address}/members", new StringContent(json.ToString(Formatting.None), Encoding.UTF8, "application/json"), ct);
    }

    /// <summary>
    /// Updates a mailing list member with given properties.  Won't touch the property if it's not passed in.
    /// </summary>
    /// <param name="mailingListAddress">The mailing list's email address.</param>
    /// <param name="memberAddress">The member's email address.</param>
    /// <param name="member">A mailing list member.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> UpdateMailingListMember(MailAddress mailingListAddress, MailAddress memberAddress, IMember member, CancellationToken ct = default(CancellationToken))
    {
      if (mailingListAddress == null)
      {
        throw new ArgumentNullException("Mailing List Address cannot be null or empty!");
      }

      if (memberAddress == null)
      {
        throw new ArgumentNullException("Member Address cannot be null or empty!");
      }

      if (member == null)
      {
        throw new ArgumentNullException("Member object cannot be null or empty!");
      }

      var formContent = new FormUrlEncodedContent(member.ToFormContent());

      return this.httpClient.PutAsync($"lists/{mailingListAddress.Address}/members/{memberAddress.Address}", formContent, ct);
    }

    /// <summary>
    /// Delete a mailing list member.
    /// </summary>
    /// <param name="mailingListAddress">The mailing list's email address.</param>
    /// <param name="memberAddress">The member's email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> DeleteMailingListMember(MailAddress mailingListAddress, MailAddress memberAddress, CancellationToken ct = default(CancellationToken))
    {
      if (mailingListAddress == null)
      {
        throw new ArgumentNullException("Mailing List Address cannot be null or empty!");
      }

      if (memberAddress == null)
      {
        throw new ArgumentNullException("Member Address cannot be null or empty!");
      }

      return this.httpClient.DeleteAsync($"lists/{mailingListAddress.Address}/members/{memberAddress.Address}", ct);
    }

    /// <summary>
    /// Returns a list of domains under your account in JSON.
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="skip">Number of records to skip.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetDomains(int limit = 100, int skip = 0, CancellationToken ct = default(CancellationToken))
    {
      if (limit < 1)
      {
        throw new ArgumentOutOfRangeException("Limit cannot be an integer value less than 1!");
      }

      if (skip < 0)
      {
        throw new ArgumentOutOfRangeException("Skip cannot be an integer value less than 0!");
      }

      return this.httpClient.GetAsync($"/domains?limit={limit}&skip={skip}", ct);
    }

    /// <summary>
    /// Returns a single domain, including credentials and DNS records.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetDomain(Uri name, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      return this.httpClient.GetAsync($"/domains/{hostname}", ct);
    }

    /// <summary>
    /// Verifies and returns a single domain, including credentials and DNS records.
    ///
    /// If the domain is successfully verified the message should be the following:  "Domain DNS records have been updated."
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetAndVerifyDomain(Uri name, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      return this.httpClient.GetAsync($"/domains/{hostname}/verify", ct);
    }

    /// <summary>
    /// Create a new domain.
    /// </summary>
    /// <param name="domain">The domain object to be created.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> AddDomain(IDomainRequest domain, CancellationToken ct = default(CancellationToken))
    {
      if (domain == null)
      {
        throw new ArgumentNullException("Domain cannot be null or empty!");
      }

      var formContent = new FormUrlEncodedContent(domain.ToFormContent());

      return this.httpClient.PostAsync("/domains", formContent, ct);
    }

    /// <summary>
    /// Delete a domain from your account.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> DeleteDomain(Uri name, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      return this.httpClient.DeleteAsync($"/domains/{hostname}", ct);
    }

    /// <summary>
    /// Returns a list of SMTP credentials for the defined domain.
    /// </summary>
    /// <<param name="name">The domain name.</param>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="skip">Number of records to skip.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetDomainCredentials(Uri name, int limit = 100, int skip = 0, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      if (limit < 1)
      {
        throw new ArgumentOutOfRangeException("Limit cannot be an integer value less than 1!");
      }

      if (skip < 0)
      {
        throw new ArgumentOutOfRangeException("Skip cannot be an integer value less than 0!");
      }

      var hostname = name.GetHostname();

      return this.httpClient.GetAsync($"/domains/{hostname}/credentials?limit={limit}&skip={skip}", ct);
    }

    /// <summary>
    /// Creates a new set of SMTP credentials for the defined domain.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="credential">The SMTP credentials to be added to the specified domain.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> AddDomainCredential(Uri name, IDomainCredentialRequest credential, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      if (credential == null)
      {
        throw new ArgumentNullException("Credential cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      var formContent = new FormUrlEncodedContent(credential.ToFormContent());

      return this.httpClient.PostAsync($"/domains/{hostname}/credentials", formContent, ct);
    }

    /// <summary>
    /// Updates the specified SMTP credentials. Currently only the password can be changed.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="username">The username to find the domain credentials with.</param>
    /// <param name="password">The password to change.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> UpdateDomainCredentialPassword(Uri name, string username, string password, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      if (username.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Username cannot be null or empty!");
      }

      if (password.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Password cannot be null or empty!");
      }

      if (checkPasswordLengthRequirement(password))
      {
        throw new ArgumentOutOfRangeException("Password must have a minimum length of 5, and maximum length of 32!");
      }

      var hostname = name.GetHostname();

      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("password", password)
      };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"/domains/{hostname}/credentials/{username}", formContent, ct);
    }

    /// <summary>
    /// Check the characteristics of the password to make sure it is within the minimum and maximum limits of length.
    /// </summary>
    /// <param name="password">The password to be checked.</param>
    /// <returns>True if the password is within the range of the minimum and maximum allowable password length.</returns>
    protected bool checkPasswordLengthRequirement(string password)
    {
      var length = password.Length;

      return (length >= MIN_SMTP_PASSWORD_LENGTH && length <= MAX_SMTP_PASSWORD_LENGTH);
    }

    /// <summary>
    /// Deletes the defined SMTP credentials.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="username">The username of the credentials to be removed.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> DeleteDomainCredential(Uri name, string username, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      return this.httpClient.DeleteAsync($"/domains/{hostname}/credentials/{username}", ct);
    }

    /// <summary>
    /// Returns delivery connection settings for the defined domain.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetDomainDeliveryConnectionSettings(Uri name, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      return this.httpClient.GetAsync($"/domains/{hostname}/connection", ct);
    }

    /// <summary>
    /// Updates the specified delivery connection settings for the defined domain.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="requireTLS">
    /// True, this requires the message only be sent over a TLS connection; false, Mailgun will still try
    /// and upgrade the connection, but if Mailgun cannot, the message will be delivered over plaintext SMTP connection.
    /// </param>
    /// <param name="skipVerification">
    /// True, message sent will not verify certificate or hostname when establishing TLS connection to send;
    /// False, Mailgun will verify certificate and hostname. If either one cannot be verified, a TLS connection will not be established.
    /// </param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> UpdateDomainDeliveryConnectionSettings(Uri name, bool requireTLS = false, bool skipVerification = false, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("require_tls", requireTLS.ToString().ToLower()),
        new KeyValuePair<string, string>("skip_verification", skipVerification.ToString().ToLower())
      };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"/domains/{hostname}/connection", formContent, ct);
    }

    /// <summary>
    /// Returns tracking settings for a domain.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetDomainTrackingSettings(Uri name, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      return this.httpClient.GetAsync($"/domains/{hostname}/tracking", ct);
    }

    /// <summary>
    /// Updates the open tracking settings for a domain.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="active">True, enable; false, disable.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> UpdateDomainOpenTrackingSettings(Uri name, bool active, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("active", active.ToYesNo()),
      };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.GetAsync($"/domains/{hostname}/tracking/open", ct);
    }

    /// <summary>
    /// Updates the click tracking settings for a domain.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="active">True, enable; false, disable.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> UpdateDomainClickTrackingSettings(Uri name, DomainClickTrackingActive active, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      var activeName = EnumLookup.GetDomainClickTrackingActiveName(active);

      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("active", activeName)
      };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"/domains/{hostname}/tracking/click", formContent, ct);
    }

    /// <summary>
    /// Updates unsubscribe tracking settings for a domain.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="active">True, enable; false, disable.</param>
    /// <param name="customHtmlFooter">Custom HTML version of unsubscribe footer.</param>
    /// <param name="customTextFooter">Custom text version of the unsubscribe footer.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> UpdateDomainUnsubscribeTrackingSettings(Uri name, bool active, string customHtmlFooter, string customTextFooter, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("active", active.ToString().ToLower()),
        new KeyValuePair<string, string>("html_footer", customHtmlFooter),
        new KeyValuePair<string, string>("text_footer", customTextFooter)
      };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"/domains/{hostname}/tracking/unsubscribe", formContent, ct);
    }

    /// <summary>
    /// Change the DKIM authority for a domain.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="self">
    /// True, the domain will be DKIM authority for itself;
    /// false, will be the same DKIM Authority as the root domain registered.
    /// </param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> ChangeDomainDKIMAuthority(Uri name, bool self, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("self", self.ToString().ToLower())
      };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"/domains/{hostname}/dkim_authority", formContent, ct);
    }
  }
}