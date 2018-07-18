using System;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MailgunSharp.MailingLists;
using MailgunSharp.Domains;
using MailgunSharp.Enums;

namespace MailgunSharp
{
  public interface IBaseMailgunService
  {
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
    /// Paginate over mailing lists under your account.
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetMailingLists(int limit = 100, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Returns a single mailing list by a given address.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetMailingList(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Creates a new mailing list.
    /// </summary>
    /// <param name="mailingList">The mailing list to be added.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddMailingList(IMailingList mailingList, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Update mailnig list properties, such as address, description or name.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="mailingList">The mailing list to be updated.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> UpdateMailingList(MailAddress address, IMailingList mailingList, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Deletes a mailing list.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteMailingList(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Paginate over list of members in the given mailing list.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="subscribed">True, lists subscribed; false, for list unsubscribed. If null will list all.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetMailingListMembers(MailAddress address, int limit = 100, bool? subscribed = null, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Retrieves a mailing list member.
    /// </summary>
    /// <param name="mailingListAddress">The mailing list's email address.</param>
    /// <param name="memberAddress">The member's email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetMailingListMember(MailAddress mailingListAddress, MailAddress memberAddress, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Adds a member to the mailing list.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="member">A mailing list member.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddMailingListMember(MailAddress address, IMember member, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Adds multiple members, up to 1,000 per call, to a Mailing list.
    /// </summary>
    /// <param name="address">Valid email address.</param>
    /// <param name="members">List of members to be added to the mailing list.</param>
    /// <param name="upsert">True, to update existing members; false, to ignore duplicates.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddMailingListMembers(MailAddress address, ICollection<IMember> members, bool upsert, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Updates a mailing list member with given properties.  Won't touch the property if it's not passed in.
    /// </summary>
    /// <param name="mailingListAddress">The mailing list's email address.</param>
    /// <param name="memberAddress">The member's email address.</param>
    /// <param name="member">A mailing list member.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> UpdateMailingListMember(MailAddress mailingListAddress, MailAddress memberAddress, IMember member, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Delete a mailing list member.
    /// </summary>
    /// <param name="mailingListAddress">The mailing list's email address.</param>
    /// <param name="memberAddress">The member's email address.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteMailingListMember(MailAddress mailingListAddress, MailAddress memberAddress, CancellationToken ct = default(CancellationToken));


    /// <summary>
    /// Returns a list of domains under your account in JSON.
    /// </summary>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="skip">Number of records to skip.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetDomains(int limit = 100, int skip = 0, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Returns a single domain, including credentials and DNS records.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetDomain(Uri name, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Verifies and returns a single domain, including credentials and DNS records.
    ///
    /// If the domain is successfully verified the message should be the following:  "Domain DNS records have been updated."
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetAndVerifyDomain(Uri name, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Create a new domain.
    /// </summary>
    /// <param name="domain">The domain object to be created.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddDomain(IDomainRequest domain, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Delete a domain from your account.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteDomain(Uri name, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Returns a list of SMTP credentials for the defined domain.
    /// </summary>
    /// <<param name="name">The domain name.</param>
    /// <param name="limit">Number of entries to return.</param>
    /// <param name="skip">Number of records to skip.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetDomainCredentials(Uri name, int limit = 100, int skip = 0, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Creates a new set of SMTP credentials for the defined domain.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="credential">The SMTP credentials to be added to the specified domain.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> AddDomainCredential(Uri name, IDomainCredentialRequest credential, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Updates the specified SMTP credentials. Currently only the password can be changed.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="username">The username to find the domain credentials with.</param>
    /// <param name="password">The password to change.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> UpdateDomainCredentialPassword(Uri name, string username, string password, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Deletes the defined SMTP credentials.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="username">The username of the credentials to be removed.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> DeleteDomainCredential(Uri name, string username, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Returns delivery connection settings for the defined domain.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetDomainDeliveryConnectionSettings(Uri name, CancellationToken ct = default(CancellationToken));

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
    Task<HttpResponseMessage> UpdateDomainDeliveryConnectionSettings(Uri name, bool requireTLS = false, bool skipVerification = false, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Returns tracking settings for a domain.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> GetDomainTrackingSettings(Uri name, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Updates the open tracking settings for a domain.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="active">True, enable; false, disable.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> UpdateDomainOpenTrackingSettings(Uri name, bool active, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Updates the click tracking settings for a domain.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="active">True, enable; false, disable.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> UpdateDomainClickTrackingSettings(Uri name, DomainClickTrackingActive active, CancellationToken ct = default(CancellationToken));

    /// <summary>
    /// Updates unsubscribe tracking settings for a domain.
    /// </summary>
    /// <param name="name">The domain name.</param>
    /// <param name="active">True, enable; false, disable.</param>
    /// <param name="customHtmlFooter">Custom HTML version of unsubscribe footer.</param>
    /// <param name="customTextFooter">Custom text version of the unsubscribe footer.</param>
    /// <param name="ct">The async task's cancellation token that will become aware of the caller cancelling the task and will terminate.</param>
    /// <returns>An async Task with the http response message.</returns>
    Task<HttpResponseMessage> UpdateDomainUnsubscribeTrackingSettings(Uri name, bool active, string customHtmlFooter, string customTextFooter, CancellationToken ct = default(CancellationToken));

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
    Task<HttpResponseMessage> ChangeDomainDKIMAuthority(Uri name, bool self, CancellationToken ct = default(CancellationToken));
  }
}