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
    ///
    /// </summary>
    /// <param name="message"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> SendMessageAsync(IMessage message, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="validateMailbox"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> ValidateEmailAddressAsync(MailAddress address, bool validateMailbox, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="syntaxOnly"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> ParseEmailAddressAsync(IRecipient recipient, bool syntaxOnly = true, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="recipients"></param>
    /// <param name="syntaxOnly"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> ParseEmailAddressesAsync(ICollection<IRecipient> recipients, bool syntaxOnly = true, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="onlyShowDedicated"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetIPsAsync(bool onlyShowDedicated = false, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="ipV4Address"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetIPDetailsAsync(string ipV4Address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetDomainsIPsAsync(CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="ipV4Address"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> AddIpAsync(string ipV4Address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="ipV4Address"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> DeleteIpAsync(string ipV4Address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetTags(CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetTag(string tagName, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="description"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> UpdateTagDescription(string tagName, string description, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> DeleteTag(string tagName, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetTagStats(string tagName, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetListCountriesStatsByTag(string tagName, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetListEmailProviderStatsByTag(string tagName, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetListDeviceStatsByTag(string tagName, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetStatTotals(CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="limit"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetBounces(int limit = 100, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetBounce(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="bounce"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> AddBounce(IBounceRequest bounce, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="bounces"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> AddBounces(ICollection<IBounceRequest> bounces, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> DeleteBounce(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> DeleteBounces(CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="limit"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetUnsubscribers(int limit = 100, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetUnsubscriber(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="unsubscriber"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> AddUnsubscriber(IUnsubscriberRequest unsubscriber, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="unsubscribers"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> AddUnsubscribers(ICollection<IUnsubscriberRequest> unsubscribers, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="tag"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> DeleteUnsubscriber(MailAddress address, string tag = "", CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="limit"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetComplaints(int limit = 100, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetComplaint(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="complaint"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> AddComplaint(IComplaintRequest complaint, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="complaints"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> AddComplaints(ICollection<IComplaintRequest> complaints, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> DeleteComplaint(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="limit"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetMailingLists(int limit = 100, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetMailingList(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="mailingList"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> AddMailingList(IMailingList mailingList, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="mailingList"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> UpdateMailingList(MailAddress address, IMailingList mailingList, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> DeleteMailingList(MailAddress address, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="limit"></param>
    /// <param name="subscribed"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetMailingListMembers(MailAddress address, int limit = 100, bool? subscribed = null, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="mailingListAddress"></param>
    /// <param name="memberAddress"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetMailingListMembers(MailAddress mailingListAddress, MailAddress memberAddress, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="member"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> AddMailingListMember(MailAddress address, IMember member, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    /// <param name="members"></param>
    /// <param name="upsert"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> AddMailingListMembers(MailAddress address, ICollection<IMember> members, bool upsert, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="mailingListAddress"></param>
    /// <param name="memberAddress"></param>
    /// <param name="member"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> UpdateMailingListMember(MailAddress mailingListAddress, MailAddress memberAddress, IMember member, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="mailingListAddress"></param>
    /// <param name="memberAddress"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> DeleteMailingListMember(MailAddress mailingListAddress, MailAddress memberAddress, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="limit"></param>
    /// <param name="skip"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetDomains(int limit = 100, int skip = 0, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetDomain(Uri name, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetAndVerifyDomain(Uri name, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> AddDomain(IDomainRequest domain, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> DeleteDomain(Uri name, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="limit"></param>
    /// <param name="skip"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetDomainCredentials(Uri name, int limit = 100, int skip = 0, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="credential"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> AddDomainCredential(Uri name, IDomainCredentialRequest credential, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> UpdateDomainCredentialPassword(Uri name, string username, string password, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="username"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> DeleteDomainCredential(Uri name, string username, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetDomainDeliveryConnectionSettings(Uri name, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="requireTLS"></param>
    /// <param name="skipVerification"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> UpdateDomainDeliveryConnectionSettings(Uri name, bool requireTLS = false, bool skipVerification = false, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetDomainTrackingSettings(Uri name, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="active"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> UpdateDomainOpenTrackingSettings(Uri name, bool active, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="active"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> UpdateDomainClickTrackingSettings(Uri name, DomainClickTrackingActive active, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="active"></param>
    /// <param name="customHtmlFooter"></param>
    /// <param name="customTextFooter"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> UpdateDomainUnsubscribeTrackingSettings(Uri name, bool active, string customHtmlFooter, string customTextFooter, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="self"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> ChangeDomainDKIMAuthority(Uri name, bool self, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="statsRequest"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetStatsTotal(IStatsRequest statsRequest, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="eventRequest"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetEvents(IEventRequest eventRequest, CancellationToken ct = default(CancellationToken));

    /// <summary>
    ///
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> GetPage(Uri uri, CancellationToken ct = default(CancellationToken));
  }
}
