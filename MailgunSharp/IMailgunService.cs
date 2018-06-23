using System;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MailgunSharp.Messages;
using MailgunSharp.Supression;

namespace MailgunSharp
{
  public interface IMailgunService
  {
    Task<HttpResponseMessage> SendMessageAsync(IMessage message, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> ValidateEmailAddressAsync(MailAddress address, bool validateMailbox, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> ParseEmailAddressAsync(IRecipient recipient, bool syntaxOnly = true, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> ParseEmailAddressesAsync(ICollection<IRecipient> recipients, bool syntaxOnly = true, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetIPsAsync(bool onlyShowDedicated = false, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetIPDetailsAsync(string ipV4Address, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetDomainsIPsAsync(CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> AddIpAsync(string ipV4Address, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> DeleteIpAsync(string ipV4Address, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetTags(CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetTag(string tagName, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> UpdateTagDescription(string tagName, string description, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> DeleteTag(string tagName, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetTagStats(string tagName, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetListCountriesStatsByTag(string tagName, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetListEmailProviderStatsByTag(string tagName, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetListDeviceStatsByTag(string tagName, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetStatTotals(CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetBounces(int limit = 100, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetBounce(MailAddress address, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> AddBounce(IBounceRequest bounce, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> AddBounces(ICollection<IBounceRequest> bounces, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> DeleteBounce(MailAddress address, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> DeleteBounces(CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetUnsubscribers(int limit = 100, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetUnsubscriber(MailAddress address, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> AddUnsubscriber(IUnsubscriberRequest unsubscriber, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> AddUnsubscribers(ICollection<IUnsubscriberRequest> unsubscribers, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> DeleteUnsubscriber(MailAddress address, string tag = "", CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetComplaints(int limit = 100, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> GetComplaint(MailAddress address, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> AddComplaint(IComplaintRequest complaint, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> AddComplaints(ICollection<IComplaintRequest> complaints, CancellationToken ct = default(CancellationToken));
    Task<HttpResponseMessage> DeleteComplaint(MailAddress address, CancellationToken ct = default(CancellationToken));
  }
}
