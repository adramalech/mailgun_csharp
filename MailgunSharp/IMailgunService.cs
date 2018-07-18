using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MailgunSharp.Messages;
using MailgunSharp.Supression;
using MailgunSharp.MailingLists;
using MailgunSharp.Domains;
using MailgunSharp.Events;
using MailgunSharp.Stats;
using MailgunSharp.Enums;

namespace MailgunSharp
{
  public interface IMailgunService
  {
    /// <summary>
    /// Send a message by passing the constructed message. Mailgun will build a MIME representation of the message and send it.
    /// </summary>
    /// <param name="message">The email message object to be sent in the request to Mailgun.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> SendMessageAsync(IMessage message, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Returns a list of IPs currently assigned to the domain.
    /// </summary>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetDomainsIPsAsync(CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Assign a dedicated IP to the domain.
    /// </summary>
    /// <param name="ipV4Address">The correctly formatted IP v4 address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddIpAsync(string ipV4Address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///  Unassign an IP from the domain specified.
    /// </summary>
    /// <param name="ipV4Address">The correctly formatted IP v4 address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteIpAsync(string ipV4Address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Return a list of tags for the domain. Providees pagination urls if the result set is too long to be returned in a single response.
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetTags(int limit = 100, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Return a given tag.
    /// </summary>
    /// <param name="tagName">The tag to be returned.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetTag(string tagName, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Updates a given tag with the information provided.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="description">Optional description of the tag.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> UpdateTagDescription(string tagName, string description, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Delete the tag.
    ///
    /// Note: The statistics for the tag are not destroyed.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteTag(string tagName, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Returns statistics for a given tag.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetTagStats(string tagName, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Returns a list of countries of origin for a given domain for different event types.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetListCountriesStatsByTag(string tagName, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Returns a list of email providers for a given domain for different event types.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetListEmailProviderStatsByTag(string tagName, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Returns a list of devices for the domain that have triggered event types.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetListDeviceStatsByTag(string tagName, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Paginate over a list of bounces for a domain.
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetBounces(int limit = 100, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Fetch a single bounce event by a given email address.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetBounce(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Add a bounce record to the bounce list. Updates the existing record if the address is alraedy there.
    /// </summary>
    /// <param name="bounce">The bounce record to add or update.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddBounce(IBounceRequest bounce, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Add multiple bounce records to the bounce list in a single API call.
    /// </summary>
    /// <param name="bounces"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddBounces(ICollection<IBounceRequest> bounces, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Clears a given bounce event. The delivery to the deleted email address resumes until it bounces again.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteBounce(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Clears all bounced email addresses for a domain. Delivery to the deleted email addresses will no longer be suppressed.
    /// </summary>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteBounces(CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Paginate over a list of unsubscribers for a domain.  Will be returned in alphabetical order.
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetUnsubscribers(int limit = 100, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Fetch a single unsubscribe record.
    /// </summary>
    /// <param name="address">The valid email address matching an unsubscriber record.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetUnsubscriber(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Add an address to the unsubscriber table.
    /// </summary>
    /// <param name="unsubscriber"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddUnsubscriber(IUnsubscriberRequest unsubscriber, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Add multiple unsubscribe records to the unsubscribe list in a single API call.
    /// </summary>
    /// <param name="unsubscribers"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddUnsubscribers(ICollection<IUnsubscriberRequest> unsubscribers, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Remove an address from the unsubscribers list. If tag parameter is not provided, completely removes an address from the list.
    /// If tag is provided will remove just that tag.
    /// </summary>
    /// <param name="address">The address of the unsubscribed person.</param>
    /// <param name="tag">The tag the unsubscriber is wanting to remove.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteUnsubscriber(MailAddress address, string tag = "", CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Paginate over a list of complaints for a domain.
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetComplaints(int limit = 100, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Fetch a single spam complaint by a given email address.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetComplaint(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Add an address to the complaints list.
    /// </summary>
    /// <param name="complaint"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddComplaint(IComplaintRequest complaint, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Add multiple complaint records to the complaint list in a single API call.
    /// </summary>
    /// <param name="complaints">Multiple complaint records.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddComplaints(ICollection<IComplaintRequest> complaints, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Remove a given spam complaint.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteComplaint(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Returns total stats for the domain.
    /// </summary>
    /// <param name="statsRequest">The request params for grabbing filtered stats items.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetStatsTotal(IStatsRequest statsRequest, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Get a list of event records based on parameters that filter the returned set of records.
    /// </summary>
    /// <param name="eventRequest">The event request params for grabbing filtered set of event records.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetEvents(IEventRequest eventRequest, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Grab a page of results from given pagination object. Can be a previous, next, first, or last page.
    /// </summary>
    /// <param name="uri">The url of the page to retreive the data from.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetPage(Uri uri, CancellationToken ct = default(CancellationToken));
  }
}
