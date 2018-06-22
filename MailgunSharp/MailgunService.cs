using System;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MailgunSharp.Messages;
using MailgunSharp.Supression;
using Newtonsoft.Json;

namespace MailgunSharp
{
  public sealed class MailgunService : IMailgunService
  {
    private readonly string companyDomain;
    private const int MAX_ADDRESS_LENGTH = 8000;
    private const int MAX_BOUNCED_RECORDS_RETURNED = 10000;
    private const int MAX_BOUNCED_RECORDS = 1000;
    private const string MAILGUN_BASE_URL = @"https://api.mailgun.net/v3/";
    private HttpClient httpClient;

    public MailgunService(string companyDomain, string apiKey, HttpClient httpClient = null)
    {
      if (checkStringIfNullOrEmpty(companyDomain))
      {
        throw new ArgumentNullException("Company domain cannot be null!");
      }

      if (checkStringIfNullOrEmpty(apiKey))
      {
        throw new ArgumentNullException("Api key cannot be null!");
      }

      this.httpClient = (httpClient == null) ? new HttpClient() : httpClient;

      this.companyDomain = companyDomain;

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

    public Task<HttpResponseMessage> ValidateEmailAddressAsync(string address, bool validateMailbox, CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.GetAsync($"address/private/validate?address={address}&mailbox_verification={validateMailbox.ToString()}", ct);
    }

    public Task<HttpResponseMessage> ParseEmailAddressAsync(IRecipient recipient, bool syntaxOnly = true, CancellationToken ct = default(CancellationToken))
    {
      if (recipient == null)
      {
        throw new ArgumentNullException("Recipients cannot be null or empty!");
      }

      var address = recipient.ToFormattedNameAddress();

      return parseAddressesAsync(address, syntaxOnly, ct);
    }

    public Task<HttpResponseMessage> ParseEmailAddressesAsync(ICollection<IRecipient> recipients, bool syntaxOnly = true, CancellationToken ct = default(CancellationToken))
    {
      if (recipients == null || recipients.Count < 1)
      {
        throw new ArgumentNullException("Recipients cannot be null or empty!");
      }

      var addresses = new StringBuilder();
      var i = 1;
      var totalMinusLast = recipients.Count - 1;

      foreach(var recipient in recipients)
      {
        var str = recipient.ToFormattedNameAddress();

        if (i < totalMinusLast)
        {
          str += ",";
        }

        addresses.Append(str);

        i++;
      }

      var addressList = addresses.ToString();

      return parseAddressesAsync(addressList, syntaxOnly, ct);
    }

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
      var ipAddress = IPAddress.Parse(ipV4Address);

      return this.httpClient.GetAsync($"ips/{ipV4Address}", ct);
    }

    public Task<HttpResponseMessage> GetDomainsIPsAsync(CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.GetAsync($"{this.companyDomain}/ips", ct);
    }

    public Task<HttpResponseMessage> AddIpAsync(string ipV4Address, CancellationToken ct = default(CancellationToken))
    {
      var ipAddress = IPAddress.Parse(ipV4Address);

      var contents = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("ip", ipV4Address) };

      var formContent = new FormUrlEncodedContent(contents);

      return this.httpClient.PostAsync($"{this.companyDomain}/ips", formContent, ct);
    }

    public Task<HttpResponseMessage> DeleteIpAsync(string ipV4Address, CancellationToken ct = default(CancellationToken))
    {
      var ipAddress = IPAddress.Parse(ipV4Address);

      return this.httpClient.DeleteAsync($"{this.companyDomain}/ips/{ipV4Address}", ct);
    }

    public Task<HttpResponseMessage> GetTags(CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.GetAsync($"{this.companyDomain}/tags", ct);
    }

    public Task<HttpResponseMessage> GetTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (checkStringIfNullOrEmpty(tagName))
      {
        throw new ArgumentNullException("Tag name cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}", ct);
    }

    public Task<HttpResponseMessage> UpdateTagDescription(string tagName, string description, CancellationToken ct = default(CancellationToken))
    {
      if (checkStringIfNullOrEmpty(tagName))
      {
        throw new ArgumentNullException("Tag name cannot be null or empty!");
      }

      var content = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("description", description) };

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PutAsync($"{this.companyDomain}/tags/{tagName}", formContent, ct);
    }

    public Task<HttpResponseMessage> DeleteTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (checkStringIfNullOrEmpty(tagName))
      {
        throw new ArgumentNullException("Tag name cannot be null or empty!");
      }

      return this.httpClient.DeleteAsync($"{this.companyDomain}/tags/{tagName}", ct);
    }

    public Task<HttpResponseMessage> GetTagStats(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (checkStringIfNullOrEmpty(tagName))
      {
        throw new ArgumentNullException("Tag name cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}/stats", ct);
    }

    public Task<HttpResponseMessage> GetListCountriesStatsByTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (checkStringIfNullOrEmpty(tagName))
      {
        throw new ArgumentNullException("Tag name cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}/stats/aggregates/countries", ct);
    }

    public Task<HttpResponseMessage> GetListEmailProviderStatsByTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (checkStringIfNullOrEmpty(tagName))
      {
        throw new ArgumentNullException("Tag name cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}/stats/aggregates/providers", ct);
    }

    public Task<HttpResponseMessage> GetListDeviceStatsByTag(string tagName, CancellationToken ct = default(CancellationToken))
    {
      if (checkStringIfNullOrEmpty(tagName))
      {
        throw new ArgumentNullException("Tag name cannot be null or empty!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/tags/{tagName}/stats/aggregates/devices", ct);
    }

    public Task<HttpResponseMessage> GetStatTotals(CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.GetAsync($"{this.companyDomain}/stats/total", ct);
    }

    public Task<HttpResponseMessage> GetBounces(int limit = 100, CancellationToken ct = default(CancellationToken))
    {
      if (limit > MAX_BOUNCED_RECORDS_RETURNED)
      {
        throw new ArgumentOutOfRangeException("Limit of records returned has a maximum limit of 10,000 records!");
      }

      return this.httpClient.GetAsync($"{this.companyDomain}/bounces?limit={limit}", ct);
    }

    public Task<HttpResponseMessage> GetBounce(string address, CancellationToken ct = default(CancellationToken))
    {
      var emailAddress = new System.Net.Mail.MailAddress(address);

      return this.httpClient.GetAsync($"{this.companyDomain}/bounces/{address}", ct);
    }

    public Task<HttpResponseMessage> AddBounce(IBounceRecord bounceRecord, CancellationToken ct = default(CancellationToken))
    {
      var emailAddress = new MailAddress(bounceRecord.Address);

      var code = (int)bounceRecord.Code;

      var content = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("address", bounceRecord.Address),
        new KeyValuePair<string, string>("code", code.ToString())
      };

      if (!checkStringIfNullOrEmpty(bounceRecord.Error))
      {
        content.Add(new KeyValuePair<string, string>("error", bounceRecord.Error));
      }

      if (bounceRecord.CreatedAt != null && bounceRecord.CreatedAt.HasValue)
      {
        content.Add(new KeyValuePair<string, string>("created_at", ((DateTimeOffset)bounceRecord.CreatedAt.Value).ToUnixTimeSeconds().ToString()));
      }

      var formContent = new FormUrlEncodedContent(content);

      return this.httpClient.PostAsync($"{this.companyDomain}/bounces", formContent, ct);
    }

    public Task<HttpResponseMessage> AddBounces(ICollection<IBounceRecord> bounceRecords, CancellationToken ct = default(CancellationToken))
    {
      if (bounceRecords == null)
      {
        throw new ArgumentNullException("Bounce records cannot be null or empty!");
      }

      if (bounceRecords.Count > MAX_BOUNCED_RECORDS)
      {
        throw new ArgumentNullException("Bounce Records cannot exceed maximum limit of 1,000 records!");
      }

      var resultList = new List<BounceRecordRequest>();

      foreach (var bounceRecord in bounceRecords)
      {
        var emailAddress = new MailAddress(bounceRecord.Address);

        var code = (int)bounceRecord.Code;

        var bounceRecordRequestObj = new BounceRecordRequest()
        {
          code = (int)bounceRecord.Code,
          address = bounceRecord.Address
        };

        if (!checkStringIfNullOrEmpty(bounceRecord.Error))
        {
          bounceRecordRequestObj.error = bounceRecord.Error;
        }

        if (bounceRecord.CreatedAt != null && bounceRecord.CreatedAt.HasValue)
        {
          bounceRecordRequestObj.created_at = ((DateTimeOffset)bounceRecord.CreatedAt.Value).ToUnixTimeSeconds();
        }

        resultList.Add(bounceRecordRequestObj);
      }

      return this.httpClient.PostAsync($"{this.companyDomain}/bounces", new StringContent(JsonConvert.SerializeObject(resultList), Encoding.UTF8, "application/json"), ct);
    }

    public Task<HttpResponseMessage> DeleteBounce(string address, CancellationToken ct = default(CancellationToken))
    {
      var emailAddress = new MailAddress(address);

      return this.httpClient.DeleteAsync($"{this.companyDomain}/bounces/{address}", ct);
    }

    public Task<HttpResponseMessage> DeleteBounces(CancellationToken ct = default(CancellationToken))
    {
      return this.httpClient.DeleteAsync($"{this.companyDomain}/bounces", ct);
    }

    private bool checkStringIfNullOrEmpty(string str)
    {
      return (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
    }
  }
}
