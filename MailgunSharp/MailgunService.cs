using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Net;
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
    public MailgunService(Uri companyDomain, string apiKey, HttpClient httpClient = null)
    {
      if (companyDomain == null)
      {
        throw new ArgumentNullException("Company domain cannot be null!");
      }

      if (apiKey.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Api key cannot be null!");
      }

      this.httpClient = (httpClient == null) ? new HttpClient() : httpClient;

      this.companyDomain = companyDomain.GetHostname();

      this.httpClient.BaseAddress = new Uri(MAILGUN_BASE_URL);

      this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{apiKey}")));
    }

    public Task<HttpResponseMessage> SendMessageAsync(IMessage message, CancellationToken ct = default(CancellationToken))
    {
      if (message == null)
      {
        throw new ArgumentNullException("Message cannot be null or empty!");
      }

      return this.httpClient.PostAsync($"{this.companyDomain}/messages", message.AsFormContent(), ct);
    }

    public Task<HttpResponseMessage> ValidateEmailAddressAsync(MailAddress address, bool validateMailbox = false, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"address/private/validate?address={address.Address}&mailbox_verification={validateMailbox.ToString()}", ct);
    }

    public Task<HttpResponseMessage> ParseEmailAddressAsync(string address, bool syntaxOnly = true, CancellationToken ct = default(CancellationToken))
    {
      if (address.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Recipients cannot be null or empty!");
      }

      return parseAddressesAsync(address, syntaxOnly, ct);
    }

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

    public Task<HttpResponseMessage> GetIPsAsync(bool onlyShowDedicated = false, CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.GetAsync($"ips?dedicated={onlyShowDedicated.ToString()}", ct);
    }

    public Task<HttpResponseMessage> GetIPDetailsAsync(string ipV4Address, CancellationToken ct = default(CancellationToken))
    {
      if (!isIPv4AddressValid(ipV4Address))
      {
        throw new FormatException("IP V4 Address is incorrectly formatted, must be in the format ###.###.###.###!");
      }

      return this.httpClient.GetAsync($"ips/{ipV4Address}", ct);
    }

    public Task<HttpResponseMessage> GetDomainsIPsAsync(CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.GetAsync($"{this.companyDomain}/ips", ct);
    }

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

    public Task<HttpResponseMessage> DeleteIpAsync(string ipV4Address, CancellationToken ct = default(CancellationToken))
    {
      if (!isIPv4AddressValid(ipV4Address))
      {
        throw new FormatException("IP V4 Address is incorrectly formatted, must be in the format ###.###.###.###!");
      }

      return this.httpClient.DeleteAsync($"{this.companyDomain}/ips/{ipV4Address}", ct);
    }

    public Task<HttpResponseMessage> GetTags(int limit = 100, CancellationToken ct = default(CancellationToken))
    {
      if (limit < 1)
      {
        throw new ArgumentOutOfRangeException("The limit of returned tags cannot be less than 1.");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags?limit={limit}", ct);
    }

    public Task<HttpResponseMessage> GetTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (doesTagNameHaveCorrectFormatting(tagName))
      {
        throw new ArgumentNullException("Tag name cannot be null or empty. Must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}", ct);
    }

    public Task<HttpResponseMessage> UpdateTagDescription(string tagName, string description, CancellationToken ct = default(CancellationToken))
    {
      if (doesTagNameHaveCorrectFormatting(tagName))
      {
        throw new ArgumentNullException("Tag name cannot be null or empty. Must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      var content = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("description", description) };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"{this.companyDomain}/tags/{tagName}", formContent, ct);
    }

    public Task<HttpResponseMessage> DeleteTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (doesTagNameHaveCorrectFormatting(tagName))
      {
        throw new ArgumentNullException("Tag name cannot be null or empty. Must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      return this.httpClient.DeleteAsync($"{this.companyDomain}/tags/{tagName}", ct);
    }

    public Task<HttpResponseMessage> GetTagStats(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (doesTagNameHaveCorrectFormatting(tagName))
      {
        throw new ArgumentNullException("Tag name cannot be null or empty. Must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}/stats", ct);
    }

    public Task<HttpResponseMessage> GetListCountriesStatsByTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (doesTagNameHaveCorrectFormatting(tagName))
      {
        throw new ArgumentNullException("Tag name cannot be null or empty. Must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}/stats/aggregates/countries", ct);
    }

    public Task<HttpResponseMessage> GetListEmailProviderStatsByTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (doesTagNameHaveCorrectFormatting(tagName))
      {
        throw new ArgumentNullException("Tag name cannot be null or empty. Must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}/stats/aggregates/providers", ct);
    }

    public Task<HttpResponseMessage> GetListDeviceStatsByTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (doesTagNameHaveCorrectFormatting(tagName))
      {
        throw new ArgumentNullException("Tag name cannot be null or empty. Must be ASCII characters with a length no greater than 128 ASCII characters!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}/stats/aggregates/devices", ct);
    }

    public Task<HttpResponseMessage> GetStatTotals(CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.GetAsync($"{this.companyDomain}/stats/total", ct);
    }

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

    public Task<HttpResponseMessage> GetBounce(MailAddress address, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/bounces/{address.Address}", ct);
    }

    public Task<HttpResponseMessage> AddBounce(IBounceRequest bounce, CancellationToken ct = default(CancellationToken))
    {
      if (bounce == null)
      {
        throw new ArgumentNullException("Bounce object cannot be null or empty!");
      }

      var formContent = new FormUrlEncodedContent(bounce.ToFormContent());

      return this.httpClient.PostAsync($"{this.companyDomain}/bounces", formContent, ct);
    }

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

    public Task<HttpResponseMessage> DeleteBounce(MailAddress address, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.DeleteAsync($"{this.companyDomain}/bounces/{address.Address}", ct);
    }

    public Task<HttpResponseMessage> DeleteBounces(CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.DeleteAsync($"{this.companyDomain}/bounces", ct);
    }

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

    public Task<HttpResponseMessage> GetUnsubscriber(MailAddress address, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/unsubscribes/{address}", ct);
    }

    public Task<HttpResponseMessage> AddUnsubscriber(IUnsubscriberRequest unsubscriber, CancellationToken ct = default(CancellationToken))
    {
      if (unsubscriber == null)
      {
        throw new ArgumentNullException("Unsubscriber object cannot be null or empty!");
      }

      var formContent = new FormUrlEncodedContent(unsubscriber.ToFormContent());

      return this.httpClient.PostAsync($"{this.companyDomain}/unsubscribes", formContent, ct);
    }

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

    public Task<HttpResponseMessage> DeleteUnsubscriber(MailAddress address, string tag = "", CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      var url = (tag.IsNullEmptyWhitespace()) ? $"{this.companyDomain}/unsubscribers/{address.Address}" : $"{this.companyDomain}/unsubscribers/{address.Address}?tag={tag}";

      return this.httpClient.DeleteAsync(url, ct);
    }

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

    public Task<HttpResponseMessage> GetComplaint(MailAddress address, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/complaints/{address.Address}", ct);
    }

    public Task<HttpResponseMessage> AddComplaint(IComplaintRequest complaint, CancellationToken ct = default(CancellationToken))
    {
      if (complaint == null)
      {
        throw new ArgumentNullException("Unsubscriber object cannot be null or empty!");
      }

      var formContent = new FormUrlEncodedContent(complaint.ToFormContent());

      return this.httpClient.PostAsync($"{this.companyDomain}/complaints", formContent, ct);
    }

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

    public Task<HttpResponseMessage> DeleteComplaint(MailAddress address, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.DeleteAsync($"{this.companyDomain}/complaints/{address.Address}", ct);
    }

    public Task<HttpResponseMessage> GetMailingLists(int limit = 100, CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.GetAsync($"lists/pages?limit={limit}", ct);
    }

    public Task<HttpResponseMessage> GetMailingList(MailAddress address, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.DeleteAsync($"lists/{address.Address}", ct);
    }

    public Task<HttpResponseMessage> AddMailingList(IMailingList mailingList, CancellationToken ct = default(CancellationToken))
    {
      if (mailingList == null)
      {
        throw new ArgumentNullException("Mailing List object cannot be null or empty!");
      }

      var formContent = new FormUrlEncodedContent(mailingList.ToFormContent());

      return this.httpClient.PostAsync("lists", formContent, ct);
    }

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

    public Task<HttpResponseMessage> DeleteMailingList(MailAddress address, CancellationToken ct = default(CancellationToken))
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      return this.httpClient.DeleteAsync($"lists/{address.Address}", ct);
    }

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

    public Task<HttpResponseMessage> GetMailingListMembers(MailAddress mailingListAddress, MailAddress memberAddress, CancellationToken ct = default(CancellationToken))
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

    public Task<HttpResponseMessage> GetDomain(Uri name, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      return this.httpClient.GetAsync($"/domains/{hostname}", ct);
    }

    public Task<HttpResponseMessage> GetAndVerifyDomain(Uri name, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      return this.httpClient.GetAsync($"/domains/{hostname}/verify", ct);
    }

    public Task<HttpResponseMessage> AddDomain(IDomainRequest domain, CancellationToken ct = default(CancellationToken))
    {
      if (domain == null)
      {
        throw new ArgumentNullException("Domain cannot be null or empty!");
      }

      var formContent = new FormUrlEncodedContent(domain.ToFormContent());

      return this.httpClient.PostAsync("/domains", formContent, ct);
    }

    public Task<HttpResponseMessage> DeleteDomain(Uri name, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      return this.httpClient.DeleteAsync($"/domains/{hostname}", ct);
    }

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

    public Task<HttpResponseMessage> DeleteDomainCredential(Uri name, string username, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      return this.httpClient.DeleteAsync($"/domains/{hostname}/credentials/{username}", ct);
    }

    public Task<HttpResponseMessage> GetDomainDeliveryConnectionSettings(Uri name, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      return this.httpClient.GetAsync($"/domains/{hostname}/connection", ct);
    }

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

    public Task<HttpResponseMessage> GetDomainTrackingSettings(Uri name, CancellationToken ct = default(CancellationToken))
    {
      if (name == null)
      {
        throw new ArgumentNullException("Name cannot be null or empty!");
      }

      var hostname = name.GetHostname();

      return this.httpClient.GetAsync($"/domains/{hostname}/tracking", ct);
    }

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

    public Task<HttpResponseMessage> GetStatsTotal(IStatsRequest statsRequest, CancellationToken ct = default(CancellationToken))
    {
      if (statsRequest == null)
      {
        throw new ArgumentNullException("Stats Request object cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"/{this.companyDomain}/stats/total?{statsRequest.ToQueryString()}", ct);
    }

    public Task<HttpResponseMessage> GetEvents(IEventRequest eventRequest, CancellationToken ct = default(CancellationToken))
    {
      if (eventRequest == null)
      {
        throw new ArgumentNullException("Event Request object cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"/{this.companyDomain}/events?{eventRequest.ToQueryString()}", ct);
    }

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
    ///
    /// </summary>
    /// <param name="ipV4Address"></param>
    /// <returns></returns>
    private bool isIPv4AddressValid(string ipV4Address)
    {
      return Regex.IsMatch(ipV4Address, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
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
  }
}
