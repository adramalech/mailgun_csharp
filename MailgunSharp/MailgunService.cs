using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Net.Mail;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MailgunSharp.Messages;
using MailgunSharp.Supression;
using MailgunSharp.MailingLists;
using MailgunSharp.Domains;
using MailgunSharp.Events;
using MailgunSharp.Stats;
using MailgunSharp.Enums;
using MailgunSharp.Extensions;
using MailgunSharp.Routes;
using MailgunSharp.Webhooks;

namespace MailgunSharp
{
  public sealed class MailgunService : IMailgunService
  {
    /// <summary>
    /// The maximum string length of a list of email addresses to be used in a email address parse validation request.
    /// </summary>
    private const int MAX_ADDRESS_LENGTH = 8000;

    /// <summary>
    /// The maximum number of records that can be returned in a response of a supression api request.
    /// </summary>
    private const int MAX_RECORD_LIMIT = 10000;

    /// <summary>
    /// The maximum integer value of an ASCII character.
    /// </summary>
    private const int MAX_ASCII_LENGTH = 128;

    /// <summary>
    /// The maximum character length of any tag.
    /// </summary>
    private const int MAX_TAG_CHAR_LENGTH = 128;

    /// <summary>
    /// The maximum number of json objects that can be added in a request.
    /// </summary>
    private const int MAX_JSON_OBJECTS = 1000;

    /// <summary>
    /// The base url for the mailgun v3 api.
    /// </summary>
    private const string MAILGUN_BASE_URL = @"https://api.mailgun.net/v3/";

    /// <summary>
    /// Maximum allowed SMTP password length.
    /// </summary>
    private const int MAX_SMTP_PASSWORD_LENGTH = 32;

    /// <summary>
    /// Minimum allowed SMTP password length.
    /// </summary>
    private const int MIN_SMTP_PASSWORD_LENGTH = 5;

    /// <summary>
    /// The maximum allowed number of urls to be attached to the webhook.
    /// </summary>
    private const int MAX_URL_COUNT = 3;

    /// <summary>
    /// The company's domain name registered in the Mailgun Account.
    /// </summary>
    private readonly string companyDomain;

    /// <summary>
    /// The http client instance to be used to make http requests to mailgun's api.
    /// </summary>
    private readonly HttpClient httpClient;

    /// <summary>
    /// Create an instance of the Mailgun Service.
    /// </summary>
    /// <param name="companyDomain">The company's domain name registered in the mailgun account.</param>
    /// <param name="apiKey">The company's mailgun account apikey.</param>
    /// <param name="httpClient">The httpclient can be optionally passed in to use one given instead of generating a new one.</param>
    public MailgunService(string companyDomain, string apiKey, HttpClient httpClient = null)
    {
      if (companyDomain == null)
      {
        throw new ArgumentNullException("Company domain cannot be null!");
      }

      if (apiKey.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Api key cannot be null!");
      }

      if (!isHostnameValid(companyDomain))
      {
        throw new FormatException("Hostname is incorrectly formatted!");
      }

      this.httpClient = httpClient ?? new HttpClient();

      this.companyDomain = companyDomain;

      this.httpClient.BaseAddress = new Uri(MAILGUN_BASE_URL);

      this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{apiKey}")));
    }

    /// <summary>
    /// Send a message by passing the constructed message. Mailgun will build a MIME representation of the message and send it.
    /// </summary>
    /// <param name="message">The email message object to be sent in the request to Mailgun.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> SendMessageAsync(IMessage message, CancellationToken ct = default(CancellationToken))
    {
      if (message == null)
      {
        throw new ArgumentNullException("Message cannot be null or empty!");
      }

      return this.httpClient.PostAsync($"{this.companyDomain}/messages", message.AsFormContent(), ct);
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
    private Task<HttpResponseMessage> parseAddressesAsync(string addressList, bool syntaxOnly, CancellationToken ct)
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
    /// Returns a list of IPs currently assigned to the domain.
    /// </summary>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetDomainsIPsAsync(CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.GetAsync($"{this.companyDomain}/ips", ct);
    }

    /// <summary>
    /// Assign a dedicated IP to the domain.
    /// </summary>
    /// <param name="ipV4Address">The correctly formatted IP v4 address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> AddIpAsync(string ipV4Address, CancellationToken ct = default(CancellationToken))
    {
      if (!isIPv4AddressValid(ipV4Address))
      {
        throw new FormatException("IP V4 Address is incorrectly formatted, must be in the format ###.###.###.###!");
      }

      var contents = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("ip", ipV4Address) };

      var formContent = new FormUrlEncodedContent(contents);

      return this.httpClient.PostAsync($"{this.companyDomain}/ips", formContent, ct);
    }

    /// <summary>
    ///  Unassign an IP from the domain specified.
    /// </summary>
    /// <param name="ipV4Address">The correctly formatted IP v4 address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> DeleteIpAsync(string ipV4Address, CancellationToken ct = default(CancellationToken))
    {
      if (!isIPv4AddressValid(ipV4Address))
      {
        throw new FormatException("IP V4 Address is incorrectly formatted, must be in the format ###.###.###.###!");
      }

      return this.httpClient.DeleteAsync($"{this.companyDomain}/ips/{ipV4Address}", ct);
    }

    /// <summary>
    /// Return a list of tags for the domain. Providees pagination urls if the result set is too long to be returned in a single response.
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetTags(int limit = 100, CancellationToken ct = default(CancellationToken))
    {
      if (limit < 1)
      {
        throw new ArgumentOutOfRangeException("The limit of returned tags cannot be less than 1.");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags?limit={limit}", ct);
    }

    /// <summary>
    /// Return a given tag.
    /// </summary>
    /// <param name="tagName">The tag to be returned.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (tagName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Tag cannot be null or empty!");
      }

      if (doesTagNameHaveCorrectFormatting(tagName))
      {
        throw new FormatException("Tag must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}", ct);
    }

    /// <summary>
    /// Updates a given tag with the information provided.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="description">Optional description of the tag.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> UpdateTagDescription(string tagName, string description, CancellationToken ct = default(CancellationToken))
    {
      if (tagName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Tag cannot be null or empty!");
      }

      if (doesTagNameHaveCorrectFormatting(tagName))
      {
        throw new FormatException("Tag must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      var content = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("description", description) };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"{this.companyDomain}/tags/{tagName}", formContent, ct);
    }

    /// <summary>
    /// Delete the tag.
    ///
    /// Note: The statistics for the tag are not destroyed.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> DeleteTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (tagName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Tag cannot be null or empty!");
      }

      if (doesTagNameHaveCorrectFormatting(tagName))
      {
        throw new FormatException("Tag must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      return this.httpClient.DeleteAsync($"{this.companyDomain}/tags/{tagName}", ct);
    }

    /// <summary>
    /// Returns statistics for a given tag.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetTagStats(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (tagName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Tag cannot be null or empty!");
      }

      if (doesTagNameHaveCorrectFormatting(tagName))
      {
        throw new FormatException("Tag must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}/stats", ct);
    }

    /// <summary>
    /// Returns a list of countries of origin for a given domain for different event types.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetListCountriesStatsByTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (tagName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Tag cannot be null or empty!");
      }

      if (doesTagNameHaveCorrectFormatting(tagName))
      {
        throw new FormatException("Tag must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}/stats/aggregates/countries", ct);
    }

    /// <summary>
    /// Returns a list of email providers for a given domain for different event types.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetListEmailProviderStatsByTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (tagName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Tag cannot be null or empty!");
      }

      if (doesTagNameHaveCorrectFormatting(tagName))
      {
        throw new FormatException("Tag must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}/stats/aggregates/providers", ct);
    }

    /// <summary>
    /// Returns a list of devices for the domain that have triggered event types.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetListDeviceStatsByTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (tagName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Tag cannot be null or empty!");
      }

      if (doesTagNameHaveCorrectFormatting(tagName))
      {
        throw new FormatException("Tag must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}/stats/aggregates/devices", ct);
    }

    /// <summary>
    /// Paginate over a list of bounces for a domain.
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetBounces(int limit = 100, CancellationToken ct = default(CancellationToken))
    {
      if (limit < 1)
      {
        throw new ArgumentOutOfRangeException("Limit cannot be an integer value less than 1!");
      }

      if (limit > MAX_RECORD_LIMIT)
      {
        throw new ArgumentOutOfRangeException("Limit of records returned has a maximum limit of 10,000 records!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/bounces?limit={limit}", ct);
    }

    /// <summary>
    /// Fetch a single bounce event by a given email address.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetBounce(MailAddress address, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/bounces/{address.Address}", ct);
    }

    /// <summary>
    /// Add a bounce record to the bounce list. Updates the existing record if the address is alraedy there.
    /// </summary>
    /// <param name="bounce">The bounce record to add or update.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> AddBounce(IBounceRequest bounce, CancellationToken ct = default(CancellationToken))
    {
      if (bounce == null)
      {
        throw new ArgumentNullException("Bounce object cannot be null or empty!");
      }

      var formContent = new FormUrlEncodedContent(bounce.ToFormContent());

      return this.httpClient.PostAsync($"{this.companyDomain}/bounces", formContent, ct);
    }

    /// <summary>
    /// Add multiple bounce records to the bounce list in a single API call.
    /// </summary>
    /// <param name="bounces"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> AddBounces(ICollection<IBounceRequest> bounces, CancellationToken ct = default(CancellationToken))
    {
      if (bounces == null)
      {
        throw new ArgumentNullException("Bounce records cannot be null or empty!");
      }

      if (bounces.Count > MAX_JSON_OBJECTS)
      {
        throw new ArgumentNullException("Bounce Records cannot exceed maximum limit of 1,000 records!");
      }

      var json = new JArray();

      foreach(var bounce in bounces)
      {
        json.Add(bounce.ToJson());
      }

      return this.httpClient.PostAsync($"{this.companyDomain}/bounces", new StringContent(json.ToString(Formatting.None), Encoding.UTF8, "application/json"), ct);
    }

    /// <summary>
    /// Clears a given bounce event. The delivery to the deleted email address resumes until it bounces again.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> DeleteBounce(MailAddress address, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.DeleteAsync($"{this.companyDomain}/bounces/{address.Address}", ct);
    }

    /// <summary>
    /// Clears all bounced email addresses for a domain. Delivery to the deleted email addresses will no longer be suppressed.
    /// </summary>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> DeleteBounces(CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.DeleteAsync($"{this.companyDomain}/bounces", ct);
    }

    /// <summary>
    /// Paginate over a list of unsubscribers for a domain.  Will be returned in alphabetical order.
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetUnsubscribers(int limit = 100, CancellationToken ct = default(CancellationToken))
    {
      if (limit < 1)
      {
        throw new ArgumentOutOfRangeException("Limit cannot be an integer value less than 1!");
      }

      if (limit > MAX_RECORD_LIMIT)
      {
        throw new ArgumentOutOfRangeException("Limit of records returned has a maximum limit of 10,000 records!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/unsubscribes?limit={limit}", ct);
    }

    /// <summary>
    /// Fetch a single unsubscribe record.
    /// </summary>
    /// <param name="address">The valid email address matching an unsubscriber record.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetUnsubscriber(MailAddress address, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/unsubscribes/{address}", ct);
    }

    /// <summary>
    /// Add an address to the unsubscriber table.
    /// </summary>
    /// <param name="unsubscriber"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> AddUnsubscriber(IUnsubscriberRequest unsubscriber, CancellationToken ct = default(CancellationToken))
    {
      if (unsubscriber == null)
      {
        throw new ArgumentNullException("Unsubscriber object cannot be null or empty!");
      }

      var formContent = new FormUrlEncodedContent(unsubscriber.ToFormContent());

      return this.httpClient.PostAsync($"{this.companyDomain}/unsubscribes", formContent, ct);
    }

    /// <summary>
    /// Add multiple unsubscribe records to the unsubscribe list in a single API call.
    /// </summary>
    /// <param name="unsubscribers"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> AddUnsubscribers(ICollection<IUnsubscriberRequest> unsubscribers, CancellationToken ct = default(CancellationToken))
    {
      if (unsubscribers == null)
      {
        throw new ArgumentNullException("Unsubscribers object cannot be null or empty!");
      }

      if (unsubscribers.Count > MAX_JSON_OBJECTS)
      {
        throw new ArgumentNullException("Unsubscribers object cannot exceed maximum limit of 1,000 records!");
      }

      var json = new JArray();

      foreach(var unsubscriber in unsubscribers)
      {
        json.Add(unsubscriber.ToJson());
      }

      return this.httpClient.PostAsync($"{this.companyDomain}/unsubscribers", new StringContent(json.ToString(Formatting.None), Encoding.UTF8, "application/json"), ct);
    }

    /// <summary>
    /// Remove an address from the unsubscribers list. If tag parameter is not provided, completely removes an address from the list.
    /// If tag is provided will remove just that tag.
    /// </summary>
    /// <param name="address">The address of the unsubscribed person.</param>
    /// <param name="tag">The tag the unsubscriber is wanting to remove.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> DeleteUnsubscriber(MailAddress address, string tag = "", CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      if (tag.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Tag cannot be null or empty!");
      }

      if (doesTagNameHaveCorrectFormatting(tag))
      {
        throw new FormatException("Tag must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      var url = (tag.IsNullEmptyWhitespace()) ? $"{this.companyDomain}/unsubscribers/{address.Address}" : $"{this.companyDomain}/unsubscribers/{address.Address}?tag={tag}";

      return this.httpClient.DeleteAsync(url, ct);
    }

    /// <summary>
    /// Paginate over a list of complaints for a domain.
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetComplaints(int limit = 100, CancellationToken ct = default(CancellationToken))
    {
      if (limit < 1)
      {
        throw new ArgumentOutOfRangeException("Limit cannot be an integer value less than 1!");
      }

      if (limit > MAX_RECORD_LIMIT)
      {
        throw new ArgumentOutOfRangeException("Limit of records returned has a maximum limit of 10,000 records!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/complaints?limit={limit}", ct);
    }

    /// <summary>
    /// Fetch a single spam complaint by a given email address.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetComplaint(MailAddress address, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/complaints/{address.Address}", ct);
    }

    /// <summary>
    /// Add an address to the complaints list.
    /// </summary>
    /// <param name="complaint"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> AddComplaint(IComplaintRequest complaint, CancellationToken ct = default(CancellationToken))
    {
      if (complaint == null)
      {
        throw new ArgumentNullException("Unsubscriber object cannot be null or empty!");
      }

      var formContent = new FormUrlEncodedContent(complaint.ToFormContent());

      return this.httpClient.PostAsync($"{this.companyDomain}/complaints", formContent, ct);
    }

    /// <summary>
    /// Add multiple complaint records to the complaint list in a single API call.
    /// </summary>
    /// <param name="complaints">Multiple complaint records.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> AddComplaints(ICollection<IComplaintRequest> complaints, CancellationToken ct = default(CancellationToken))
    {
      if (complaints == null)
      {
        throw new ArgumentNullException("Complaints object cannot be null or empty!");
      }

      if (complaints.Count > MAX_JSON_OBJECTS)
      {
        throw new ArgumentNullException("Complaints object cannot exceed maximum limit of 1,000 records!");
      }

      var json = new JArray();

      foreach(var complaint in complaints)
      {
        json.Add(complaint.ToJson());
      }

      return this.httpClient.PostAsync($"{this.companyDomain}/complaints", new StringContent(json.ToString(Formatting.None), Encoding.UTF8, "application/json"), ct);
    }

    /// <summary>
    /// Remove a given spam complaint.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> DeleteComplaint(MailAddress address, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.DeleteAsync($"{this.companyDomain}/complaints/{address.Address}", ct);
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
    /// <param name="domainName">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetDomain(string domainName, CancellationToken ct = default(CancellationToken))
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("DomainName cannot be ");
      }

      if (!isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
      }

      return this.httpClient.GetAsync($"/domains/{domainName}", ct);
    }

    /// <summary>
    /// Verifies and returns a single domain, including credentials and DNS records.
    ///
    /// If the domain is successfully verified the message should be the following:  "Domain DNS records have been updated."
    /// </summary>
    /// <param name="domainName">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetAndVerifyDomain(string domainName, CancellationToken ct = default(CancellationToken))
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("DomainName cannot be ");
      }

      if (!isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
      }

      return this.httpClient.GetAsync($"/domains/{domainName}/verify", ct);
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
    /// <param name="domainName">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> DeleteDomain(string domainName, CancellationToken ct = default(CancellationToken))
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("DomainName cannot be ");
      }

      if (!isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
      }

      return this.httpClient.DeleteAsync($"/domains/{domainName}", ct);
    }

    /// <summary>
    /// Returns a list of SMTP credentials for the defined domain.
    /// </summary>
    /// <<param name="domainName">The domain name.</param>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="skip">Number of records to skip.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetDomainCredentials(string domainName, int limit = 100, int skip = 0, CancellationToken ct = default(CancellationToken))
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("DomainName cannot be ");
      }

      if (!isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
      }

      if (limit < 1)
      {
        throw new ArgumentOutOfRangeException("Limit cannot be an integer value less than 1!");
      }

      if (skip < 0)
      {
        throw new ArgumentOutOfRangeException("Skip cannot be an integer value less than 0!");
      }

      return this.httpClient.GetAsync($"/domains/{domainName}/credentials?limit={limit}&skip={skip}", ct);
    }

    /// <summary>
    /// Creates a new set of SMTP credentials for the defined domain.
    /// </summary>
    /// <param name="domainName">The domain name.</param>
    /// <param name="credential">The SMTP credentials to be added to the specified domain.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> AddDomainCredential(string domainName, IDomainCredentialRequest credential, CancellationToken ct = default(CancellationToken))
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("DomainName cannot be ");
      }

      if (!isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
      }

      if (credential == null)
      {
        throw new ArgumentNullException("Credential cannot be null or empty!");
      }

      var formContent = new FormUrlEncodedContent(credential.ToFormContent());

      return this.httpClient.PostAsync($"/domains/{domainName}/credentials", formContent, ct);
    }

    /// <summary>
    /// Updates the specified SMTP credentials. Currently only the password can be changed.
    /// </summary>
    /// <param name="domainName">The domain name.</param>
    /// <param name="username">The username to find the domain credentials with.</param>
    /// <param name="password">The password to change.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> UpdateDomainCredentialPassword(string domainName, string username, string password, CancellationToken ct = default(CancellationToken))
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("DomainName cannot be ");
      }

      if (!isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
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

      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("password", password)
      };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"/domains/{domainName}/credentials/{username}", formContent, ct);
    }

    /// <summary>
    /// Check the characteristics of the password to make sure it is within the minimum and maximum limits of length.
    /// </summary>
    /// <param name="password">The password to be checked.</param>
    /// <returns>True if the password is within the range of the minimum and maximum allowable password length.</returns>
    private bool checkPasswordLengthRequirement(string password)
    {
      var length = password.Length;

      return (length >= MIN_SMTP_PASSWORD_LENGTH && length <= MAX_SMTP_PASSWORD_LENGTH);
    }

    /// <summary>
    /// Deletes the defined SMTP credentials.
    /// </summary>
    /// <param name="domainName">The domain name.</param>
    /// <param name="username">The username of the credentials to be removed.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> DeleteDomainCredential(string domainName, string username, CancellationToken ct = default(CancellationToken))
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("DomainName cannot be ");
      }

      if (!isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
      }

      return this.httpClient.DeleteAsync($"/domains/{domainName}/credentials/{username}", ct);
    }

    /// <summary>
    /// Returns delivery connection settings for the defined domain.
    /// </summary>
    /// <param name="domainName">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetDomainDeliveryConnectionSettings(string domainName, CancellationToken ct = default(CancellationToken))
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("DomainName cannot be null or empty!");
      }

      if (!isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
      }

      return this.httpClient.GetAsync($"/domains/{domainName}/connection", ct);
    }

    /// <summary>
    /// Updates the specified delivery connection settings for the defined domain.
    /// </summary>
    /// <param name="domainName">The domain name.</param>
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
    public Task<HttpResponseMessage> UpdateDomainDeliveryConnectionSettings(string domainName, bool requireTLS = false, bool skipVerification = false, CancellationToken ct = default(CancellationToken))
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("DomainName cannot be ");
      }

      if (!isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
      }

      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("require_tls", requireTLS.ToString().ToLower()),
        new KeyValuePair<string, string>("skip_verification", skipVerification.ToString().ToLower())
      };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"/domains/{domainName}/connection", formContent, ct);
    }

    /// <summary>
    /// Returns tracking settings for a domain.
    /// </summary>
    /// <param name="domainName">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetDomainTrackingSettings(string domainName, CancellationToken ct = default(CancellationToken))
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("DomainName cannot be ");
      }

      if (!isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
      }

      return this.httpClient.GetAsync($"/domains/{domainName}/tracking", ct);
    }

    /// <summary>
    /// Updates the open tracking settings for a domain.
    /// </summary>
    /// <param name="domainName">The domain name.</param>
    /// <param name="active">True, enable; false, disable.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> UpdateDomainOpenTrackingSettings(string domainName, bool active, CancellationToken ct = default(CancellationToken))
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("DomainName cannot be ");
      }

      if (!isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
      }

      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("active", active.ToYesNo()),
      };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.GetAsync($"/domains/{domainName}/tracking/open", ct);
    }

    /// <summary>
    /// Updates the click tracking settings for a domain.
    /// </summary>
    /// <param name="domainName">The domain name.</param>
    /// <param name="active">True, enable; false, disable.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> UpdateDomainClickTrackingSettings(string domainName, DomainClickTrackingActive active, CancellationToken ct = default(CancellationToken))
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("DomainName cannot be ");
      }

      if (!isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
      }

      var activeName = EnumLookup.GetDomainClickTrackingActiveName(active);

      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("active", activeName)
      };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"/domains/{domainName}/tracking/click", formContent, ct);
    }

    /// <summary>
    /// Updates unsubscribe tracking settings for a domain.
    /// </summary>
    /// <param name="domainName">The domain name.</param>
    /// <param name="active">True, enable; false, disable.</param>
    /// <param name="customHtmlFooter">Custom HTML version of unsubscribe footer.</param>
    /// <param name="customTextFooter">Custom text version of the unsubscribe footer.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> UpdateDomainUnsubscribeTrackingSettings(string domainName, bool active, string customHtmlFooter, string customTextFooter, CancellationToken ct = default(CancellationToken))
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("DomainName cannot be ");
      }

      if (!isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
      }

      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("active", active.ToString().ToLower()),
        new KeyValuePair<string, string>("html_footer", customHtmlFooter),
        new KeyValuePair<string, string>("text_footer", customTextFooter)
      };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"/domains/{domainName}/tracking/unsubscribe", formContent, ct);
    }

    /// <summary>
    /// Change the DKIM authority for a domain.
    /// </summary>
    /// <param name="domainName">The domain name.</param>
    /// <param name="self">
    /// True, the domain will be DKIM authority for itself;
    /// false, will be the same DKIM Authority as the root domain registered.
    /// </param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> ChangeDomainDKIMAuthority(string domainName, bool self, CancellationToken ct = default(CancellationToken))
    {
      if (domainName.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("DomainName cannot be ");
      }

      if (!isHostnameValid(domainName))
      {
        throw new FormatException("Domain name is incorrectly formatted!");
      }

      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("self", self.ToString().ToLower())
      };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"/domains/{domainName}/dkim_authority", formContent, ct);
    }

    /// <summary>
    /// Returns total stats for the domain.
    /// </summary>
    /// <param name="statsRequest">The request params for grabbing filtered stats items.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetStatsTotal(IStatsRequest statsRequest, CancellationToken ct = default(CancellationToken))
    {
      if (statsRequest == null)
      {
        throw new ArgumentNullException("Stats Request object cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"/{this.companyDomain}/stats/total?{statsRequest.ToQueryString()}", ct);
    }

    /// <summary>
    /// Get a list of event records based on parameters that filter the returned set of records.
    /// </summary>
    /// <param name="eventRequest">The event request params for grabbing filtered set of event records.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetEvents(IEventRequest eventRequest, CancellationToken ct = default(CancellationToken))
    {
      if (eventRequest == null)
      {
        throw new ArgumentNullException("Event Request object cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"/{this.companyDomain}/events?{eventRequest.ToQueryString()}", ct);
    }

    /// <summary>
    /// Grab a page of results from given pagination object. Can be a previous, next, first, or last page.
    /// </summary>
    /// <param name="uri">The url of the page to retreive the data from.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetPage(Uri uri, CancellationToken ct = default(CancellationToken))
    {
      if (uri == null)
      {
        throw new ArgumentNullException("Uri of page cannot be null or empty!");
      }

      var url = uri.ToString().Replace($"{MAILGUN_BASE_URL}/{this.companyDomain}", "");

      return this.httpClient.GetAsync(url, ct);
    }

    /// <summary>
    /// Fetches the list of routes.
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="skip">Number of records to skip.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetRoutes(int limit = 100, int skip = 0, CancellationToken ct = default(CancellationToken))
    {
      if (limit < 1)
      {
        throw new ArgumentOutOfRangeException("Limit cannot have a value less than one!");
      }

      if (skip < 0)
      {
        throw new ArgumentOutOfRangeException("You cannot skip less than zero records!");
      }

      return this.httpClient.GetAsync($"/routes?limit={limit}&skip={skip}", ct);
    }

    /// <summary>
    /// Returns a single route object based on its ID.
    /// </summary>
    /// <param name="id">ID of the route.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetRoute(string id, CancellationToken ct = default(CancellationToken))
    {
      if (id.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Id cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"/routes/{id}", ct);
    }

    /// <summary>
    /// Create a new route.
    /// </summary>
    /// <param name="route">The route to be created.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> AddRoute(IRoute route, CancellationToken ct = default(CancellationToken))
    {
      if (route == null)
      {
        throw new ArgumentNullException("Route cannot be null or empty!");
      }

      var content = route.AsFormContent();

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PostAsync("/routes", formContent, ct);
    }

    /// <summary>
    /// Update a given route by ID.
    /// </summary>
    /// <param name="id">ID of the route.</param>
    /// <param name="route">The route to be updated.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> UpdateRoute(string id, IRoute route, CancellationToken ct = default(CancellationToken))
    {
      if (id.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Id cannot be null or empty!");
      }

      if (route == null)
      {
        throw new ArgumentNullException("Route cannot be null or empty!");
      }

      var content = route.AsFormContent();

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"/routes/{id}", formContent, ct);
    }

    /// <summary>
    /// Delete a route based on the id.
    /// </summary>
    /// <param name="id">ID of the route.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> DeleteRoute(string id, CancellationToken ct = default(CancellationToken))
    {
      if (id.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Id cannot be null or empty!");
      }

      return this.httpClient.DeleteAsync($"/routes/{id}", ct);
    }

    /// <summary>
    /// Returns a list of webhooks for the given domain.
    /// </summary>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetWebhooks(CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.GetAsync($"/domains/{this.companyDomain}/webhooks", ct);
    }

    /// <summary>
    /// Return details about the webhook specified.
    /// </summary>
    /// <param name="name">The name of the webhook.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> GetWebhookDetails(string name, CancellationToken ct = default(CancellationToken))
    {
      if (name.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Webhook name cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"/domains/{this.companyDomain}/webhooks/{name}", ct);
    }

    /// <summary>
    /// Create a new webhook.
    /// </summary>
    /// <param name="webhook">The new webhook to be created.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> CreateWebhook(IWebhook webhook, CancellationToken ct = default(CancellationToken))
    {
      if (webhook == null)
      {
        throw new ArgumentNullException("Webhook cannot be null or empty!");
      }

      var content = webhook.AsFormContent();

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PostAsync($"/domains/{this.companyDomain}/webhooks", formContent, ct);
    }

    /// <summary>
    /// Update a webhook's URLs.
    /// </summary>
    /// <param name="type">The webhook's type to be updated.</param>
    /// <param name="urls">The webhook's list of urls to be updated.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> UpdateWebhook(WebHookType type, ICollection<Uri> urls, CancellationToken ct = default(CancellationToken))
    {
      if (urls == null || urls.Count < 1)
      {
        throw new ArgumentNullException("Webhook urls cannot be null or empty!");
      }

      if (urls.Count > MAX_URL_COUNT)
      {
        throw new ArgumentNullException("Webhook urls cannot exceed maximum length of three!");
      }

      var content = new Collection<KeyValuePair<string, string>>();

      foreach (var url in urls)
      {
        content.Add("url", url.ToString());
      }

      var name = EnumLookup.GetWebhookTypeName(type);

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"/domains/{this.companyDomain}/webhooks/{name}", formContent, ct);
    }

    /// <summary>
    /// Deletes an existing webhook.
    /// </summary>
    /// <param name="name">The name of the webhook.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    public Task<HttpResponseMessage> DeleteWebhook(string name, CancellationToken ct = default(CancellationToken))
    {
      if (name.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Webhook name cannot be null or empty!");
      }

      return this.httpClient.DeleteAsync($"/domains/{this.companyDomain}/webhooks/{name}", ct);
    }

    /// <summary>
    /// Check if the IP Address is a valid IP v4 formatted address in the range of 0.0.0.0 to 255.255.255.255.
    /// </summary>
    /// <param name="ipV4Address">The ip v4 address to check.</param>
    /// <returns>True, if the ip address is valid, false if the ip address is invalid.</returns>
    private bool isIPv4AddressValid(string ipV4Address)
    {
      return Regex.IsMatch(ipV4Address, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
    }

    /// <summary>
    /// Check if the tag is not null, empty, or whitespace.
    ///
    /// In addition, check if the tag name not properly formatted ASCII and has a maximum length of 128 characters.
    /// </summary>
    /// <param name="tagName">The tag name to check.</param>
    /// <returns>True if the tag is correctly formatted and isn't null or empty; false, if the tag is not correctly formatted.</returns>
    private bool doesTagNameHaveCorrectFormatting(string tagName)
    {
      if (tagName.IsNullEmptyWhitespace() || tagName.Any(t => t > MAX_ASCII_LENGTH) || tagName.Length > MAX_TAG_CHAR_LENGTH)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Check if the hostname is valid format.
    /// </summary>
    /// <param name="hostname">The hostname to check.</param>
    /// <returns>True, if valid, false if not valid.</returns>
    private bool isHostnameValid(string hostname)
    {
      return Uri.CheckHostName(hostname) == UriHostNameType.Dns;
    }
  }
}
