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
  public sealed class MailgunService : BaseMailgunService, IMailgunService
  {
    /// <summary>
    /// The maximum integer value of an ASCII character.
    /// </summary>
    private const int MAX_ASCII_LENGTH = 128;

    /// <summary>
    /// The maximum character length of any tag.
    /// </summary>
    private const int MAX_TAG_CHAR_LENGTH = 128;

    /// <summary>
    /// The company's domain name registered in the Mailgun Account.
    /// </summary>
    private readonly string companyDomain;

    /// <summary>
    /// Create an instance of the Mailgun Service.
    /// </summary>
    /// <param name="companyDomain">The company's domain name registered in the mailgun account.</param>
    /// <param name="apiKey">The company's mailgun account apikey.</param>
    /// <param name="httpClient">The httpclient can be optionally passed in to use one given instead of generating a new one.</param>
    public MailgunService(Uri companyDomain, string apiKey, HttpClient httpClient = null) : base(apiKey, httpClient)
    {
      if (companyDomain == null)
      {
        throw new ArgumentNullException("Company domain cannot be null!");
      }

      this.companyDomain = companyDomain.GetHostname();
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
