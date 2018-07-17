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
    /// Given a arbitrary email address, validates address based off defined checks.
    ///
    /// Uses the private api call, using the private auth api key, and will not be subject to rate limits of the public api calls.
    /// </summary>
    /// <param name="address">The email address to be validated.</param>
    /// <param name="validateMailbox">True, a mailbox verification check will be performed against the address, false will not.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> ValidateEmailAddressAsync(MailAddress address, bool validateMailbox = false, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Parses a single email address. Will determine if the address is syntactically valid, and optionally pass DNS and ESP specific grammar checks.
    ///
    /// Uses the private api call, using the private auth api key, and will not be subject to rate limits of the public api calls.
    /// </summary>
    /// <param name="address">The email address to check.</param>
    /// <param name="syntaxOnly"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> ParseEmailAddressAsync(string address, bool syntaxOnly = true, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Parses a list of email addresses. Will determine if the addresses are syntactically valid, and optionally pass DNs and ESP specific grammar checks.
    ///
    /// Uses the private api call, using the private auth api key, and will not be subject to rate limits of the public api calls.
    /// </summary>
    /// <param name="addresses">The list of addresses to check.</param>
    /// <param name="syntaxOnly">True, perform only syntax checks; false, DNS and ESP specific validation as well.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> ParseEmailAddressesAsync(ICollection<string> addresses, bool syntaxOnly = true, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Returns a list of IPs assigned to your account.
    /// </summary>
    /// <param name="onlyShowDedicated">True, return only dedicated IPs; false, return all IPs.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetIPsAsync(bool onlyShowDedicated = false, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Return information about the specified IP address.
    /// </summary>
    /// <param name="ipV4Address">The correctly formatted IP v4 address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetIPDetailsAsync(string ipV4Address, CancellationToken ct = default(CancellationToken));

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
    /// Returns a list of coutn
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
    /// Returns total stats for the domain.
    /// </summary>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetStatTotals(CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetBounces(int limit = 100, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetBounce(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="bounce"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddBounce(IBounceRequest bounce, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="bounces"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddBounces(ICollection<IBounceRequest> bounces, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteBounce(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteBounces(CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetUnsubscribers(int limit = 100, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetUnsubscriber(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="unsubscriber"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddUnsubscriber(IUnsubscriberRequest unsubscriber, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="unsubscribers"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddUnsubscribers(ICollection<IUnsubscriberRequest> unsubscribers, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="tag"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteUnsubscriber(MailAddress address, string tag = "", CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetComplaints(int limit = 100, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetComplaint(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="complaint"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddComplaint(IComplaintRequest complaint, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="complaints"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddComplaints(ICollection<IComplaintRequest> complaints, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteComplaint(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetMailingLists(int limit = 100, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetMailingList(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="mailingList"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddMailingList(IMailingList mailingList, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="mailingList"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> UpdateMailingList(MailAddress address, IMailingList mailingList, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteMailingList(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="subscribed"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetMailingListMembers(MailAddress address, int limit = 100, bool? subscribed = null, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="mailingListAddress"></param>
    /// <param name="memberAddress"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetMailingListMembers(MailAddress mailingListAddress, MailAddress memberAddress, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="member"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddMailingListMember(MailAddress address, IMember member, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="members"></param>
    /// <param name="upsert"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddMailingListMembers(MailAddress address, ICollection<IMember> members, bool upsert, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="mailingListAddress"></param>
    /// <param name="memberAddress"></param>
    /// <param name="member"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> UpdateMailingListMember(MailAddress mailingListAddress, MailAddress memberAddress, IMember member, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="mailingListAddress"></param>
    /// <param name="memberAddress"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteMailingListMember(MailAddress mailingListAddress, MailAddress memberAddress, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="skip"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetDomains(int limit = 100, int skip = 0, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetDomain(Uri name, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetAndVerifyDomain(Uri name, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddDomain(IDomainRequest domain, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteDomain(Uri name, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="skip"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetDomainCredentials(Uri name, int limit = 100, int skip = 0, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="credential"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddDomainCredential(Uri name, IDomainCredentialRequest credential, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> UpdateDomainCredentialPassword(Uri name, string username, string password, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="username"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteDomainCredential(Uri name, string username, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetDomainDeliveryConnectionSettings(Uri name, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="requireTLS"></param>
    /// <param name="skipVerification"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> UpdateDomainDeliveryConnectionSettings(Uri name, bool requireTLS = false, bool skipVerification = false, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetDomainTrackingSettings(Uri name, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="active"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> UpdateDomainOpenTrackingSettings(Uri name, bool active, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="active"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> UpdateDomainClickTrackingSettings(Uri name, DomainClickTrackingActive active, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="active"></param>
    /// <param name="customHtmlFooter"></param>
    /// <param name="customTextFooter"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> UpdateDomainUnsubscribeTrackingSettings(Uri name, bool active, string customHtmlFooter, string customTextFooter, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="self"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> ChangeDomainDKIMAuthority(Uri name, bool self, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="statsRequest"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetStatsTotal(IStatsRequest statsRequest, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="eventRequest"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetEvents(IEventRequest eventRequest, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetPage(Uri uri, CancellationToken ct = default(CancellationToken));
  }
}
